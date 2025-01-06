using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data.Models;

[Index(nameof(Name))]
public class Room
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Beings that are present in this room.
    /// </summary>
    [InverseProperty(nameof(Being.InRoom))]
    public ICollection<Being> BeingsHere
    {
        get => _lazyLoader.Load(this, ref _beingsHere);
        set => _beingsHere = value;
    }

    /// <summary>
    /// Rooms that can be accessed from this room.
    /// </summary>
    public ICollection<Room> ConnectedToRooms 
    { 
        get => _lazyLoader.Load(this, ref _connectedToRooms);
        set => _connectedToRooms = value;
    }

    /// <summary>
    /// Rooms that have access to this room.
    /// </summary>
    public ICollection<Room> ConnectedFromRooms
    {
        get => _lazyLoader.Load(this, ref _connectedFromRooms);
        set => _connectedFromRooms = value;
    }

    /// <summary>
    /// Room pool to use for extending this room.
    /// </summary>
    public int? CuriosityPrimaryKey { get; set; }
    [InverseProperty(nameof(RoomPool.Curiosities))]
    public RoomPool? Curiosity
    {
        get => _lazyLoader.Load(this, ref _curiosity);
        set => _curiosity = value;
    }

    public string? Description { get; set; }

    /// <summary>
    /// Setting to determine if room can be accessed
    /// from any other room.
    /// </summary>
    public required bool GlobalAccess { get; set; }

    /// <summary>
    /// Collection of features being must meet to enter the room.
    /// </summary>
    public ICollection<Feature> RequiredFeatures
    {
        get => _lazyLoader.Load(this, ref _requiredFeatures);
        set => _requiredFeatures = value;
    }

    public required string Name { get; set; }

    /// <summary>
    /// Being that hosts this room.
    /// </summary>
    [InverseProperty(nameof(Being.RoomInside))]
    public required Being? InBeing
    {
        get => _lazyLoader.Load(this, ref _inBeing);
        set => _inBeing = value;
    }

    public int InventoryPrimaryKey { get; set; }
    public required Inventory Inventory
    {
        get => _lazyLoader.Load(this, ref _inventory);
        set => _inventory = value;
    }

    [InverseProperty(nameof(RoomPool.Prototypes))]
    public ICollection<RoomPool> RoomPools
    {
        get => _lazyLoader.Load(this, ref _roomPools);
        set => _roomPools = value;
    }

    private readonly GameContext _context;
    private readonly ILazyLoader _lazyLoader;

    private ICollection<Being> _beingsHere;
    private ICollection<Feature> _requiredFeatures;
    private ICollection<Room> _connectedFromRooms;
    private ICollection<Room> _connectedToRooms;
    private ICollection<RoomPool> _roomPools;
    private Inventory _inventory;
    private RoomPool _curiosity;
    private Being? _inBeing;

    public Room() {}

    private Room(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
    }

    public Room Clone()
    {
        var clone = new Room()
        {
            Curiosity = this.Curiosity,
            Description = this.Description,
            GlobalAccess = this.GlobalAccess,
            Inventory = this.Inventory.Clone(),
            Name = this.Name,
            InBeing = null
        };

        foreach(var feature in this.RequiredFeatures)
        {
            clone.RequiredFeatures.Add(feature);
        }

        return clone;
    }

    public void ConnectBidirectionally(Room destination)
    {
        this.ConnectedToRooms.Add(destination);
        
        destination.ConnectedToRooms.Add(this);
    }

    public async Task<CommandResult> CuriosityIs(string poolName)
    {
        var pool = await _context.FindRoomPool(poolName);

        if(pool is not null)
        {
            Curiosity = pool;

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Set($"{Name}'s curiosity", pool.Name)
            );
        }
        else
        {
            return RoomPoolDoesNotExist(poolName);
        }
    }

    public async Task<CommandResult> Expand(Being being)
    {
        if(Curiosity is null)
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage(Message.DoesNotHave(Name, "a curiosity"));
        }
        else
        {
            return await Curiosity.CreateExpansion(from: this, being: being);            
        }
    }

    public ICollection<ItemHatcher> GetItemHatchers()
    {
        return Inventory.ItemHatchers;
    } 

    /// <summary>
    /// Combines static connections, rooms inside beings,
    /// and room of a host being.
    /// </summary>
    public ICollection<Room> GetLeadsToRooms()
    {
        var leads = new List<Room>();

        foreach(var room in ConnectedToRooms)
        {
            leads.Add(room);
        }

        foreach(var being in BeingsHere)
        {
            if(being.RoomInside is not null)
            {
                leads.Add(being.RoomInside);
            }
        }

        if(InBeing is not null)
        {
            leads.Add(InBeing.InRoom);
        }

        return leads;
    }

    public async Task SetUniqueName()
    {
        if(await _context.Rooms.AnyAsync(
            room => room.Name == this.Name
        ))
        {
            this.Name += StringUtilities.GetRandomCharacter();
        }
    }

    public bool HasAccessTo(Room destination)
    {
        if(destination.GlobalAccess)
        {
            return true;
        }

        if(GetLeadsToRooms().Contains(destination))
        {
            return true;
        }

        return false;
    }

    public List<string> Show()
    {
        var texts = new List<string>();

        if(Description is not null)
        {
            texts.Add(Description);
        }
        texts.Add(GetBeingsText());
        texts.Add(GetLeadsToText());
        texts.Add(GetCuriosityText());
        if(GlobalAccess)
        {
            texts.Add($"{Name} can be accessed globally.");
        }
        texts.Add(GetInventoryText());
        texts.Add(GetItemHatchersText());
        texts.Add(GetFeaturesText());

        return texts;
    }

    private string GetBeingsText()
    {
        if(BeingsHere.Count > 0)
        {
            var beingNames = new List<string>();

            foreach(var being in BeingsHere)
            {
                beingNames.Add(being.Name);
            }

            return
            Message.List(beingNames) + " " +
            (BeingsHere.Count == 1 ? "is" : "are") +
            $" in {Name}.";
        }

        return Message.DoesNotHave(Name, "beings");
    }

    private string GetCuriosityText()
    {
        if(Curiosity is not null)
        {
            return $"{Name} has a curiosity. {Curiosity.Show()}";
        }
        else
        {
            return Message.DoesNotHave(Name, "a curiosity");
        }
    }

    private string GetLeadsToText()
    {
        var rooms = GetLeadsToRooms();

        if(rooms.Count > 0)
        {
            var roomNames = new List<string>();

            foreach(var room in rooms)
            {
                roomNames.Add(room.Name);
            }

            return $"{Name} leads to {Message.List(roomNames)}.";
        }

        return Message.DoesNotHave(Name, "leads");
    }

    private string GetInventoryText()
    {
        if(Inventory.IsEmpty)
        {
            return Message.DoesNotHave(Name, "items");
        }
        else
        {
            return
            $"{Name} has {Inventory.ItemStacks.Count} "
            + $"stacks of items: {Inventory.Contents()}.";
        }
    }

    private string GetItemHatchersText()
    {
        var hatchers = GetItemHatchers();
        if(hatchers.Count == 0)
        {
            return Message.DoesNotHave(Name, "item hatchers");
        }
        else
        {
            return
            $"{Name} is subscribed to following item hatchers: "
            + GetItemHatcherDetails() + ".";
        }
    }

    private string GetItemHatcherDetails()
    {
        var hatcherDetails = new List<string>();

        foreach(var hatcher in GetItemHatchers())
        {
            hatcherDetails.Add(
                hatcher.Show()
            );
        }

        return Message.List(hatcherDetails);
    }

    private string GetFeaturesText()
    {
        if(RequiredFeatures.Count > 0)
        {
            return $"{Name} has requirements for features: {GetFeatureNames()}.";
        }
        else
        {
            return $"{Name} does not have requirements for features.";
        }
    }

    private string GetFeatureNames()
    {
        var featureNames = new List<string>();

        foreach(var feature in RequiredFeatures)
        {
            featureNames.Add(feature.Name);
        }

        return Message.List(featureNames);
    }
}
