using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data.Models;

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

    public int InventoryPrimaryKey { get; set; }
    public required Inventory Inventory
    {
        get => _lazyLoader.Load(this, ref _inventory);
        set => _inventory = value;
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
    private Inventory _inventory;
    private Room _inRoom;
    private Room? _roomInside;
    private User _createdByUser;

    public Being() {}

    private Being(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
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
            InRoom = this.InRoom,
            Inventory = this.Inventory.Clone(),
            Name = this.Name,
            RoomInside = this.RoomInside is null ?
            null : this.RoomInside.Clone()
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
        await clone.SetUniqueName();

        await _context.Beings.AddAsync(clone);
        
        if(clone.RoomInside is not null)
        {
            await clone.RoomInside.SetUniqueName();
        }

        await _context.SaveChangesAsync();

        return clone;
    }

    public async Task<CommandResult> DeployItem(string itemName)
    {
        bool itemExists = _context.Items.Any(
            item => item.Name == itemName
        );

        if(itemExists)
        {
            var item = _context.Items.First(
                item => item.Name == itemName
            );

            if(Inventory.Contains(item, 1))
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
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.DoesNotExist("item", itemName));
        }
    }

    public async Task<CommandResult> Go(string roomName)
    {
        bool roomExists = _context.Rooms.Any(
            room => room.Name == roomName
        );

        if(roomExists)
        {
            var room = _context.Rooms.First(
                room => room.Name == roomName
            );

            return await MoveTo(room);
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.DoesNotExist("room", roomName));
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
        if(Inventory.Contains(item, quantity))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<CommandResult> MoveTo(Room destination)
    {
        if(destination == InRoom)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{Name} is in {destination.Name}.");
        }

        if(destination == RoomInside)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{Name} can not enter itself.");
        }

        if(!InRoom.HasAccessTo(destination))
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{destination.Name} can not be accessed from {InRoom.Name}."
            );
        }
        else
        {
            InRoom = destination;

            await _context.SaveChangesAsync();
            
            return new CommandResult(StatusCode.Success)
            .AddMessage($"{Name} moved in {destination.Name}.");
        }
    }

    public void RemoveItems(int quantity, Item item)
    {
        Inventory.RemoveItems(item, quantity);
    }

    public async Task SetUniqueName()
    {
        if(await _context.Beings.AnyAsync(
            being => being.Name == this.Name
        ))
        {
            this.Name += StringUtilities.GetRandomCharacter();
        }
    }

    public List<string> Show()
    {
        var texts = new List<string>();

        texts.Add(GetRoomText());
        texts.Add(GetFeaturesText());
        texts.Add(GetInsideRoomText());

        return texts;
    }

    public async Task<CommandResult> TakeItemFromRoom(string itemName)
    {
        bool itemExists = _context.Items.Any(
            item => item.Name == itemName
        );

        if(itemExists)
        {
            var item = _context.Items.First(
                item => item.Name == itemName
            );

            return await InRoom.TakeItem(
                item: item,
                being: this
            );
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.DoesNotExist("item", itemName));
        }
    }

    public async Task<CommandResult> TryBreakItem(string itemName)
    {
        bool itemExists = _context.Items.Any(
            item => item.Name == itemName
        );

        if(itemExists)
        {
            var item = _context.Items.First(
                item => item.Name == itemName
            );

            if(item.IsCraftable() && item.CraftPlan is not null)
            {
                if(Inventory.Contains(item, 1))
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
            return new CommandResult(
                StatusCode.Fail
            ).AddMessage(Message.DoesNotExist("item", itemName));
        }
    }

    public async Task<CommandResult> TryCraftItem(string itemName)
    {
        bool itemExists = _context.Items.Any(
            item => item.Name == itemName
        );

        if(itemExists)
        {
            var item = _context.Items.First(
                item => item.Name == itemName
            );

            if (item.IsCraftable() && item.CraftPlan is not null)
            {
                // Check that player has each item of craft plan
                foreach(var comp in item.CraftPlan.Components)
                {
                    if(!Inventory.Contains(comp.Item, comp.Quantity))
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
            return new CommandResult(
                StatusCode.Fail
            ).AddMessage(Message.DoesNotExist("item", itemName));
        }
    }

    private async Task<CommandResult> BreakItem(CraftPlan craftPlan)
    {
        Inventory.RemoveItems(craftPlan.Product, 1);

        foreach(var component in craftPlan.Components)
        {
            Inventory.AddItems(component.Item, component.Quantity);
        }

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{Name} breaked {Message.Quantity(craftPlan.Product.Name, 1)} to {craftPlan.MadeOf()}."
        );
    }

    private async Task<CommandResult> CraftItem(CraftPlan craftPlan)
    {
        foreach(var component in craftPlan.Components)
        {
            Inventory.RemoveItems(component.Item, component.Quantity);
        }

        Inventory.AddItems(craftPlan.Product, 1);

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            $"{Name} crafted {craftPlan.MadeOf()} to {Message.Quantity(craftPlan.Product.Name, 1)}."
        );
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
}
