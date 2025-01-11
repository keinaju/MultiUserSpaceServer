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
    /// Location of being.
    /// </summary>
    public int InRoomPrimaryKey { get; set; }
    public required Room InRoom
    {
        get => _lazyLoader.Load(this, ref _inRoom);
        set => _inRoom = value;
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

    public required string Name { get; set; }

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

    private readonly ILazyLoader _lazyLoader;
    private readonly GameContext _context;

    private ICollection<Feature> _features;
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

    public async Task<CommandResult> BeingIsFeature(string featureName)
    {
        var feature = await _context.FindFeature(featureName);

        if(feature is not null)
        {
            if(!HasFeature(feature))
            {
                Features.Add(feature);

                await _context.SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage(
                    $"{Name} now has the feature {feature.Name}."
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
        else
        {
            return FeatureDoesNotExist(featureName);
        }
    }

    public async Task<CommandResult> BeingNameIs(string beingName)
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

    public async Task<CommandResult> Explore()
    {
        return await InRoom.Expand(this);
    }

    public Being Clone()
    {
        var clone = new Being(_context, _lazyLoader)
        {
            CreatedByUser = this.CreatedByUser,
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
        clone.Name = await _context.GetUniqueBeingName(Name);

        await _context.Beings.AddAsync(clone);

        var insideRoom = clone.RoomInside;
        if(insideRoom is not null)
        {
            insideRoom.Name = await _context
            .GetUniqueRoomName(insideRoom.Name);
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
        this.FreeInventory.AddItems(item, quantity);

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{Message.Quantity(item.Name, quantity)} has been added to {Name}'s inventory."
        );
    }

    public async Task<CommandResult> MoveTo(Room destination)
    {
        if(destination == InRoom)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{this.Name} is in {destination.Name}.");
        }

        if(destination == RoomInside)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{this.Name} can not enter itself.");
        }

        if(!InRoom.HasAccessTo(destination))
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{this.Name} can not access {destination.Name} from {InRoom.Name}."
            );
        }
        else
        {
            InRoom = destination;

            await _context.SaveChangesAsync();
            
            return new CommandResult(StatusCode.Success)
            .AddMessage($"{this.Name} has moved in {destination.Name}.");
        }
    }

    public void RemoveItems(int quantity, Item item)
    {
        FreeInventory.RemoveItems(item, quantity);
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

    public async Task<CommandResult> Sell(
        int sellQuantity, int buyQuantity,
        Item sellItem, Item buyItem
    )
    {
        if(this.FreeInventory.Contains(sellItem, sellQuantity))
        {
            var newOffer = new Offer()
            {
                CreatedByBeing = this,
                ItemToBuy = buyItem,
                ItemToSell = sellItem,
                QuantityToBuy = buyQuantity,
                QuantityToSell = sellQuantity
            };

            var matchingOffer = await FindMatchingOffer(newOffer);

            if(matchingOffer is null)
            {
                // Offer can not be resolved
                
                // Transfer items to trade inventory
                await this.FreeInventory.TransferTo(
                    this.TradeInventory, sellItem, sellQuantity
                );

                // Save offer to database
                await _context.Offers.AddAsync(newOffer);
                await _context.SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage($"{Name} has made an offer: {newOffer.GetDetails()}.");
            }
            else
            {
                // Offer can be resolved

                if(newOffer.QuantityToSell > matchingOffer.QuantityToBuy)
                {
                    // The new offer can take advantage of an existing offer
                    // by decreasing it's own sell amount

                    // Adjust the new offer to match the existing offer's price
                    newOffer.QuantityToSell = matchingOffer.QuantityToBuy;
                }

                // Trade items between matching offers
                return await TradeItems(
                    offerToDelete: matchingOffer,
                    newOffer: newOffer
                );
            }
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{this.Name} does not have the required items: {Message.Quantity(sellItem.Name, sellQuantity)}.");
        }
    }

    public CommandResult Show()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessages(this.GetDetails());
    }

    public CommandResult ShowInventory()
    {
        var freeContent = this.FreeInventory.Contents();
        var tradeContent = this.TradeInventory.Contents();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{Name}'s free inventory has {(freeContent is null ? "no items" : freeContent)}."
        ).AddMessage(
            $"{Name} is trading {(tradeContent is null ? "no items" : tradeContent)}."
        );
    }

    public CommandResult ShowRoom()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessage($"{this.Name} looks at the {this.InRoom.Name}.")
        .AddMessages(this.InRoom.GetDetails());
    }
    
    public async Task<CommandResult> TakeItemFromRoom(string itemName)
    {
        var item = await _context.FindItem(itemName);

        if(item is not null)
        {
            return await InRoom.Inventory.TakeItemStack(
                item: item,
                takerInventory: this.FreeInventory,
                takerName: this.Name,
                giverName: InRoom.Name
            );
        }
        else
        {
            return ItemDoesNotExist(itemName);
        }
    }

    public async Task<CommandResult> TryBreakItem(string itemName)
    {
        var item = await _context.FindItem(itemName);

        if(item is not null)
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
                return new CommandResult(
                    StatusCode.Fail
                ).AddMessage($"{itemName} is not a craftable item.");
            }
        }
        else
        {
            return ItemDoesNotExist(itemName);
        }
    }

    public async Task<CommandResult> TryCraftItem(string itemName)
    {
        var item = await _context.FindItem(itemName);

        if(item is not null)
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
                return new CommandResult(
                    StatusCode.Fail
                ).AddMessage($"{itemName} is not a craftable item.");
            }
        }
        else
        {
            return ItemDoesNotExist(itemName);
        }
    }

    private async Task<CommandResult> BreakItem(CraftPlan craftPlan)
    {
        FreeInventory.RemoveItems(craftPlan.Product, 1);

        foreach(var component in craftPlan.Components)
        {
            FreeInventory.AddItems(component.Item, component.Quantity);
        }

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{Name} breaked {Message.Quantity(craftPlan.Product.Name, 1)} to {craftPlan.MadeOf()}."
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
            FreeInventory.RemoveItems(component.Item, component.Quantity);
        }

        FreeInventory.AddItems(craftPlan.Product, 1);

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{Name} crafted {craftPlan.MadeOf()} to {Message.Quantity(craftPlan.Product.Name, 1)}."
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

    private async Task<CommandResult> TradeItems(
        Offer offerToDelete, Offer newOffer
    )
    {
        // The old offer transfers from trade inventory to free inventory
        await offerToDelete.CreatedByBeing.TradeInventory.TransferTo(
            newOffer.CreatedByBeing.FreeInventory,
            newOffer.ItemToBuy,
            newOffer.QuantityToBuy
        );

        // The new offer transfers from free inventory to free inventory
        await newOffer.CreatedByBeing.FreeInventory.TransferTo(
            offerToDelete.CreatedByBeing.FreeInventory,
            newOffer.ItemToSell,
            newOffer.QuantityToSell
        );

        // Remove old offer from database
        _context.Offers.Remove(offerToDelete);

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage($"Transaction has been resolved: {newOffer.GetDetails()} with {offerToDelete.CreatedByBeing.Name}.");
    }
}
