using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data.Models;

[Index(nameof(Name))]
public class Being
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// User that this being belongs to.
    /// </summary>
    public int CreatedByUserPrimaryKey { get; set; }
    public required User CreatedByUser
    {
        get => _lazyLoader.Load(this, ref _createdByUser);
        set => _createdByUser = value;
    }

    /// <summary>
    /// Features associated with this being.
    /// </summary>
    public ICollection<Feature> Features
    {
        get => _lazyLoader.Load(this, ref _features);
        set => _features = value;
    }

    /// <summary>
    /// Inventory for items that are not reserved for offers.
    /// </summary>
    public int FreeInventoryPrimaryKey { get; set; }
    public required Inventory FreeInventory
    {
        get => _lazyLoader.Load(this, ref _freeInventory);
        set => _freeInventory = value;
    }

    /// <summary>
    /// Location of being.
    /// </summary>
    public int InRoomPrimaryKey { get; set; }
    public required Room InRoom
    {
        get => _lazyLoader.Load(this, ref _inRoom);
        set => _inRoom = value;
    }
    
    public required string Name { get; set; }

    /// <summary>
    /// Active offers created by this being.
    /// </summary>
    public ICollection<Offer> CreatedOffers
    {
        get => _lazyLoader.Load(this, ref _createdOffers);
        set => _createdOffers = value;
    }

    /// <summary>
    /// Optional room inside being, e.g. vehicle room.
    /// </summary>
    public int? RoomInsidePrimaryKey { get; set; }
    [InverseProperty(nameof(Room.InBeing))]
    public Room? RoomInside
    {
        get => _lazyLoader.Load(this, ref _roomInside);
        set => _roomInside = value;
    }

    /// <summary>
    /// Inventory for items that are reserved for offers,
    /// and can not be used for other activities such as crafting.
    /// Items are picked from free inventory to trade inventory
    /// when new offer is created. If the offer is resolved,
    /// the received items are placed in the free inventory.
    /// </summary>
    public int TradeInventoryPrimaryKey { get; set; }
    public required Inventory TradeInventory
    {
        get => _lazyLoader.Load(this, ref _tradeInventory);
        set => _tradeInventory = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private readonly GameContext _context;

    private ICollection<Feature> _features;
    private ICollection<Offer> _createdOffers;
    private Inventory _freeInventory;
    private Inventory _tradeInventory;
    private Room _inRoom;
    private Room? _roomInside;
    private User _createdByUser;

    public Being() {}

    private Being(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
    }

    public async Task<CommandResult> AddFeature(Feature feature)
    {
        if(!HasFeature(feature))
        {
            Features.Add(feature);

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Added("feature", feature.Name, this.Name)
            );
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{Name} already has the feature {feature.Name}."
            );
        }
    }

    public async Task<CommandResult> Explore()
    {
        return await InRoom.Expand(this);
    }

    public Being Clone()
    {
        var clone = new Being(_context, _lazyLoader)
        {
            CreatedByUser = this.CreatedByUser,
            Features = new List<Feature>(),
            FreeInventory = this.FreeInventory.Clone(),
            InRoom = this.InRoom,
            Name = this.Name,
            TradeInventory = new Inventory(),
            RoomInside = this.RoomInside is null ? null : this.RoomInside.Clone()
        };

        foreach(var feature in this.Features)
        {
            clone.Features.Add(feature);
        }

        return clone;
    }

    public async Task<Being> CreateDeployedBeing(Being prototype)
    {
        var clone = prototype.Clone();
        clone.InRoom = this.InRoom;
        clone.CreatedByUser = this.CreatedByUser;
        clone.Name = await _context.GetUniqueBeingName(prototype.Name);

        await _context.Beings.AddAsync(clone);

        var insideRoom = clone.RoomInside;
        if(insideRoom is not null)
        {
            insideRoom.Name = await _context.GetUniqueRoomName(insideRoom.Name);
        }

        await _context.SaveChangesAsync();

        return clone;
    }

    public async Task<CommandResult> DeployItem(string itemName)
    {
        var item = await _context.FindItem(itemName);

        if(item is not null)
        {
            if(FreeInventory.Contains(item, 1))
            {
                return await item.Deploy(being: this);
            }
            else
            {
                return new CommandResult(StatusCode.Fail)
                .AddMessage(
                    Message.DoesNotHave(
                        Name, Message.Quantity(item.Name, 1)
                    )
                );
            }
        }
        else
        {
            return ItemDoesNotExist(itemName);
        }
    }

    public bool HasFeature(Feature feature)
    {
        if(Features.Contains(feature))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasItems(int quantity, Item item)
    {
        if(FreeInventory.Contains(item, quantity))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<CommandResult> MakeItems(Item item, int quantity)
    {
        await this.FreeInventory.AddItems(item, quantity);

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{Message.Quantity(item.Name, quantity)} has been added to {Name}'s inventory."
        );
    }

    public async Task RemoveItems(int quantity, Item item)
    {
        await this.FreeInventory.RemoveItems(item, quantity);
    }

    public async Task<CommandResult> SetInsideRoom(Room room)
    {
        this.RoomInside = room;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set(
                $"{Name}'s inside room", room.Name
            )
        );
    }

    public List<string> GetDetails()
    {
        var texts = new List<string>();

        texts.Add(GetRoomText());
        texts.Add(GetFeaturesText());
        texts.Add(GetInsideRoomText());

        return texts;
    }


    public async Task<CommandResult> Offer(
        int sellQuantity,
        int buyQuantity,
        Item sellItem,
        Item buyItem
    )
    {
        if(!this.FreeInventory.Contains(sellItem, sellQuantity))
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{this.Name} does not have the required items: {Message.Quantity(sellItem.Name, sellQuantity)}."
            );
        }

        var newOffer = new Offer()
        {
            CreatedByBeing = this,
            ItemToBuy = buyItem,
            ItemToSell = sellItem,
            QuantityToBuy = buyQuantity,
            QuantityToSell = sellQuantity
        };
        await _context.Offers.AddAsync(newOffer);
        await _context.SaveChangesAsync();

        // Transfer items to trade inventory
        await this.FreeInventory.TransferTo(
            this.TradeInventory, sellItem, sellQuantity
        );

        var matchingOffer = await FindMatchingOffer(newOffer);

        if(matchingOffer is null)
        {
            // Offer can not be resolved

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                $"{Name} has made a new offer: {newOffer.GetDetails()}."
            );
        }
        else
        {
            // Offer can be resolved

            // Trade items between matching offers
            return await newOffer.TradeItems(matchingOffer);
        }
    }

    public async Task<CommandResult> SetInRoom(Room destination)
    {
        InRoom = destination;
        
        var moveResult = new CommandResult(StatusCode.Success)
        .AddMessage($"{this.Name} has moved in {destination.Name}.");

        await _context.SaveChangesAsync();

        // Try to match active offers created by this being
        // with offers in new room
        var resolvedOfferResults = new List<CommandResult>();
        foreach(var offer in this.CreatedOffers)
        {
            var matchingOffer = await FindMatchingOffer(offer);
            if(matchingOffer is not null)
            {
                resolvedOfferResults.Add(
                    await offer.TradeItems(matchingOffer)
                );
            }
        }
        // Get messages from successful transactions 
        if(resolvedOfferResults.Count > 0)
        {
            foreach(var result in resolvedOfferResults)
            {
                moveResult.AddMessages(result.GetMessages());
            }
        }

        return moveResult;
    }

    public async Task<CommandResult> SetName(string beingName)
    {
        var validationResult = TextSanitation.ValidateName(beingName);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        else
        {
            var cleanName = TextSanitation.GetCleanName(beingName);

            if(await _context.BeingNameIsReserved(cleanName))
            {
                return NameIsReserved("being", cleanName);
            }
            else
            {
                return await Rename(cleanName);
            }
        }
    }
    
    public CommandResult Show()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessages(this.GetDetails());
    }

    public CommandResult ShowInventory()
    {
        var result = new CommandResult(StatusCode.Success);

        var freeContent = this.FreeInventory.Contents();
        result.AddMessage(
            $"{this.Name}'s free inventory has {(freeContent is null ? "no items" : freeContent)}."
        );

        var tradeContent = this.TradeInventory.Contents();
        result.AddMessage(
            $"{this.Name}'s trade inventory has {(tradeContent is null ? "no items" : tradeContent)}."
        );

        if(this.CreatedOffers.Count > 0)
        {
            var offerDetails = new List<string>();
            foreach(var offer in this.CreatedOffers)
            {
                offerDetails.Add(offer.GetDetails());
            }

            result.AddMessage(
                $" {this.Name}'s offers are: {Message.List(offerDetails)}."
            );
        }
        else
        {
            result.AddMessage($"{this.Name} does not have offers.");
        }

        return result;
    }

    public CommandResult ShowRoom()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessage($"{this.Name} looks at the {this.InRoom.Name}.")
        .AddMessages(this.InRoom.GetDetails());
    }
    
    public async Task<CommandResult> TakeItemStackFromCurrentRoom(Item item)
    {
        return await InRoom.Inventory.TakeItemStack(
            item: item,
            takerInventory: this.FreeInventory,
            takerName: this.Name,
            giverName: InRoom.Name
        );
    }

    public async Task<CommandResult> TryBreakItem(Item item)
    {
        if(item.IsCraftable() && item.CraftPlan is not null)
        {
            if(FreeInventory.Contains(item, 1))
            {
                return await BreakItem(item.CraftPlan);
            }
            else
            {
                return new CommandResult(StatusCode.Fail)
                .AddMessage(
                    Message.DoesNotHave(
                        Name, Message.Quantity(item.Name, 1)
                    )
                );
            }
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{item.Name} is not a breakable item.");
        }
    }

    public async Task<CommandResult> TryCraftItem(Item item)
    {
        if (item.IsCraftable() && item.CraftPlan is not null)
        {
            // Check that player has each item of craft plan
            foreach(var comp in item.CraftPlan.Components)
            {
                if(!FreeInventory.Contains(comp.Item, comp.Quantity))
                {
                    return new CommandResult(StatusCode.Fail)
                    .AddMessage(
                        Message.DoesNotHave(
                            Name, Message.Quantity(comp.Item.Name, comp.Quantity)
                        )
                    );
                }
            }

            return await CraftItem(item.CraftPlan);
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{item.Name} is not a craftable item.");
        }
    }

    public async Task<CommandResult> TryToMove(Room destination, bool admin = false)
    {
        if(destination == InRoom)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"{this.Name} is in {destination.Name}.");
        }

        if(destination == RoomInside)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{this.Name} can not enter itself.");
        }

        if(!admin && !InRoom.HasAccessTo(destination))
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{this.Name} can not access {destination.Name} from {InRoom.Name}."
            );
        }
        else
        {
            return await SetInRoom(destination);
        }
    }

    private async Task<CommandResult> BreakItem(CraftPlan craftPlan)
    {
        await this.FreeInventory.RemoveItems(craftPlan.Product, 1);

        foreach(var component in craftPlan.Components)
        {
            await this.FreeInventory.AddItems(component.Item, component.Quantity);
        }

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{this.Name} breaked {Message.Quantity(craftPlan.Product.Name, 1)} to {craftPlan.MadeOf()}."
        );
    }

    private int CompareOffersByBuyQuantity(Offer a, Offer b)
    {
        if (a.QuantityToBuy > b.QuantityToBuy)
        {
            return 1;
        }

        if (a.QuantityToBuy == b.QuantityToBuy)
        {
            return 0;
        }

        return -1;
    }

    private async Task<CommandResult> CraftItem(CraftPlan craftPlan)
    {
        foreach(var component in craftPlan.Components)
        {
            await this.FreeInventory.RemoveItems(component.Item, component.Quantity);
        }

        await this.FreeInventory.AddItems(craftPlan.Product, 1);

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{this.Name} crafted {craftPlan.MadeOf()} to {Message.Quantity(craftPlan.Product.Name, 1)}."
        );
    }

    private async Task<Offer?> FindMatchingOffer(Offer newOffer)
    {
        var offers = await _context.FindMatchingOffers(newOffer);

        if(offers.Count == 0)
        {
            // Offer can not be resolved
            return null;
        }
        else
        {
            // Offer can be resolved
            // Sort to find the cheapest deal
            offers.Sort(CompareOffersByBuyQuantity);

            return offers.First();
        }
    }

    private string GetRoomText()
    {
        return $"{Name} is in {InRoom.Name}.";
    }

    private string GetFeaturesText()
    {
        if(Features.Count == 0)
        {
            return Message.DoesNotHave(
                Name, "features"
            );
        }

        var featureNames = new List<string>();

        foreach(var feature in Features)
        {
            featureNames.Add(feature.Name);
        }

        return $"{Name} has features: {Message.List(featureNames)}.";
    }

    private string GetInsideRoomText()
    {
        if(RoomInside is null)
        {
            return Message.DoesNotContain(
                Name, "an inside room"
            );
        }
        else
        {
            return Message.Contains(
                Name, $"an inside room {RoomInside.Name}"
            );
        }
    }

    private async Task<CommandResult> Rename(string newName)
    {
        var oldName = Name;

        Name = newName;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(Message.Renamed(oldName, newName));
    }
}
