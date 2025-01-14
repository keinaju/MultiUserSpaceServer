﻿using System.ComponentModel.DataAnnotations;
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
    /// Limits exploring by defining a number of static connections 
    /// from this room to other rooms.
    /// </summary>
    public required int ConnectionLimit { get; set; }

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

    public async Task<CommandResult> AddFeature(Feature feature)
    {
        if(RequiredFeatures.Contains(feature))
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{Name} already has the feature {feature.Name}."
            );
        }
        else
        {
            RequiredFeatures.Add(feature);

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                $"Feature {feature.Name} has been added to {Name}'s required features."
            );
        }
    }

    public CommandResult BeingMeetsRequiredFeatures(Being being)
    {
        if(this.RequiredFeatures.Count() == 0)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{being.Name} can not go in {this.Name} because no required features have been defined in the room.");
        }

        foreach(var requiredFeature in this.RequiredFeatures)
        {
            if(being.Features.Contains(requiredFeature))
            {
                return new CommandResult(StatusCode.Success)
                .AddMessage($"{being.Name} meets the requirements of {this.Name}.");
            }
        }

        var featureNames = new List<string>();
        foreach(var feature in this.RequiredFeatures)
        {
            featureNames.Add(feature.Name);
        }

        return new CommandResult(StatusCode.Fail)
        .AddMessage($"{being.Name} does not have any of the required features: {Message.List(featureNames)}.");
    }
    
    public Room Clone()
    {
        var clone = new Room()
        {
            ConnectionLimit = this.ConnectionLimit,
            Curiosity = this.Curiosity,
            Description = this.Description,
            GlobalAccess = this.GlobalAccess,
            InBeing = null,
            Inventory = this.Inventory.Clone(),
            Name = this.Name,
            RequiredFeatures = this.RequiredFeatures
        };

        return clone;
    }

    public void ConnectBidirectionally(Room destination)
    {
        this.ConnectedToRooms.Add(destination);
        
        destination.ConnectedToRooms.Add(this);
    }

    public async Task<CommandResult> Expand(Being being)
    {
        if(Curiosity is null)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.DoesNotHave(Name, "a curiosity"));
        }
        else
        {
            return await Curiosity.CreateExpansion(from: this, being: being);            
        }
    }
    
    public List<string> GetDetails()
    {
        var texts = new List<string>();

        if(Description is not null)
        {
            texts.Add(Description);
        }
        texts.Add(GetLeadsToText());
        if(GlobalAccess)
        {
            texts.Add($"{Name} can be accessed globally.");
        }
        texts.Add(GetCuriosityText());
        texts.Add(GetInventoryText());
        texts.Add(GetItemHatchersText());
        texts.Add(GetBeingsText());
        texts.Add(GetOfferDetailsOfBeings());
        texts.Add(GetFeaturesText());

        return texts;
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

    public async Task<CommandResult> ItemHatcherIntervalIs(
        Item item, int interval
    )
    {
        var hatcher = Inventory.ItemHatchers.SingleOrDefault(
            hatcher => hatcher.Item == item
        );
        if(hatcher is not null)
        {
            hatcher.IntervalInTicks = interval;

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Set($"item hatcher", hatcher.GetDetails())
            );
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{Name} has not subscribed to {item.Name} item hatcher."
            );
        }
    }

    public async Task<CommandResult> ItemHatcherQuantityIs(
        Item item,
        int minQuantity,
        int maxQuantity
    )
    {
        var hatcher = Inventory.ItemHatchers.SingleOrDefault(
            hatcher => hatcher.Item == item
        );

        if(hatcher is not null)
        {
            hatcher.MinimumQuantity = minQuantity;
            hatcher.MaximumQuantity = maxQuantity;

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Set($"item hatcher", hatcher.GetDetails())
            );
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"{Name} has not subscribed to {item.Name} item hatcher."
            );
        }
    }

    public async Task<CommandResult> NewItemHatcher(Item item)
    {
        var existingHatcher = this.Inventory.GetItemHatcher(item);

        if(existingHatcher is null)
        {
            var hatcher = new ItemHatcher()
            {
                IntervalInTicks = 1,
                Item = item,
                MinimumQuantity = 1,
                MaximumQuantity = 1
            };

            await _context.ItemHatchers.AddAsync(hatcher);

            hatcher.Inventories.Add(this.Inventory);

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Created($"{item.Name} item hatcher ({hatcher.GetDetails()})")
            );
        }
        else
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{Name} already has an item hatcher ({existingHatcher.GetDetails()}).");
        }
    }

    public async Task<CommandResult> SetConnectionLimit(int limit)
    {
        this.ConnectionLimit = limit;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set($"{this.Name}'s connection limit", limit.ToString())
        );
    }

    public async Task<CommandResult> SetCuriosity(RoomPool pool)
    {
        this.Curiosity = pool;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set($"{this.Name}'s curiosity", pool.Name)
        );
    }
    
    public async Task<CommandResult> SetDescription(string roomDescription)
    {
        var validationResult = TextSanitation
        .ValidateDescription(roomDescription);

        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        else
        {
            var cleanDescription = TextSanitation
            .GetCleanDescription(roomDescription);

            Description = cleanDescription;

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Set($"{Name}'s description", cleanDescription)
            );
        }
    }
    
    public async Task<CommandResult> SetGlobalAccess(bool newValue)
    {
        GlobalAccess = newValue;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set(
                $"{Name}'s global access",
                newValue.ToString().ToUpper()
            )
        );
    }
    
    public async Task<CommandResult> SetName(string newName)
    {
        var validationResult = TextSanitation.ValidateName(newName);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        else
        {
            var cleanName = TextSanitation.GetCleanName(newName);
            if(await _context.RoomNameIsReserved(cleanName))
            {
                return NameIsReserved("room", cleanName);
            }
            else
            {
                var message = Message.Renamed(Name, cleanName);

                Name = cleanName;

                await _context.SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage(message);
            }
        }
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
            return $"{Name} has a curiosity. " +
            Curiosity.GetCuriosityPresentationText();
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
                hatcher.GetDetails()
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

    private string GetOfferDetailsOfBeings()
    {
        var offersInRoom = new List<Offer>();
        foreach(var being in this.BeingsHere)
        {
            foreach(var offer in being.CreatedOffers)
            {
                offersInRoom.Add(offer);
            }
        }

        if(offersInRoom.Count == 0)
        {
            return $"{this.Name} has no offers.";
        }
        else
        {
            return $"{this.Name} has offers: {GetOfferDetails(offersInRoom)}.";
        }
    }

    private string GetOfferDetails(IEnumerable<Offer> offers)
    {
        var offerDetails = new List<string>();

        foreach(var offer in offers)
        {
            offerDetails.Add(offer.GetDetails());
        }

        return Message.List(offerDetails);
    }
}
