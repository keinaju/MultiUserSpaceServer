using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class RoomPool
{
    [Key]
    public int PrimaryKey { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Item that is needed to explore this room pool.
    /// Null if no item is needed to explore.
    /// </summary>
    public int? FeeItemPrimaryKey { get; set; }
    public required Item? FeeItem
    {
        get => _lazyLoader.Load(this, ref _feeItem);
        set => _feeItem = value;
    }

    public required string Name { get; set; }

    /// <summary>
    /// Rooms that are used as prototypes to create
    /// new rooms.
    /// </summary>
    [InverseProperty(nameof(Room.RoomPools))]
    public ICollection<Room> Prototypes
    {
        get => _lazyLoader.Load(this, ref _prototypes);
        set => _prototypes = value;
    }

    /// <summary>
    /// Rooms that use this room pool as a curiosity.
    /// </summary>
    [InverseProperty(nameof(Room.Curiosity))]
    public ICollection<Room> Curiosities
    {
        get => _lazyLoader.Load(this, ref _curiosities);
        set => _curiosities = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private readonly GameContext _context;

    private Item? _feeItem;
    private ICollection<Room> _prototypes;
    private ICollection<Room> _curiosities;

    public RoomPool() {}

    private RoomPool(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
    }

    public async Task<CommandResult> CreateExpansion(Room from)
    {
        if(Prototypes.Count == 0)
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage(Message.DoesNotHave(Name, "prototypes"));
        }
        else
        {
            int randomIndex = new Random().Next(0, Prototypes.Count);
            var randomPrototype = Prototypes.ToArray()[randomIndex];
            var expansion = randomPrototype.Clone();
            await _context.Rooms.AddAsync(expansion);
            await expansion.SetUniqueName();
            expansion.ConnectBidirectionally(from);
            await _context.SaveChangesAsync();

            return new CommandResult(
                CommandResult.StatusCode.Success
            ).AddMessage($"{expansion.Name} has been found.");
        }

    }

    public bool HasRoom(Room room)
    {
        foreach(var prototype in Prototypes)
        {
            if (prototype == room)
            {
                return true;
            }
        }

        return false;
    }

    public ICollection<string> GetDetails()
    {
        return new List<string>
        {
            GetDescriptionText(),
            GetFeeItemText(),
            GetPrototypesText(),
            GetCuriosityCountText()
        };
    }

    public string Show()
    {
        var texts = new List<string>();

        if(Description is not null)
        {
            texts.Add(Description);
        }

        if(FeeItem is not null)
        {
            texts.Add(
                $" Exploring this requires {Message.Quantity(FeeItem.Name, 1)}."
            );
        }

        return string.Join(" ", texts);
    }

    private string GetDescriptionText()
    {
        if(Description is null)
        {
            return Message.DoesNotHave(Name, "a description");
        }
        else
        {
            return  $"{Name} has a description '{Description}'.";
        }
    }

    private string GetFeeItemText()
    {
        if(FeeItem is null)
        {
            return Message.DoesNotHave(Name, "a fee item");
        }
        else
        {
            return $"{Name}'s fee item is {FeeItem.Name}.";
        }
    }

    private string GetPrototypesText()
    {
        if(Prototypes.Count == 0)
        {
            return Message.DoesNotHave(Name, "prototype rooms");
        }
        else
        {
            var roomNames = new List<string>();

            foreach(var room in Prototypes)
            {
                roomNames.Add(room.Name);
            }

            return $"{Name} uses following rooms as prototypes: {Message.List(roomNames)}.";
        }
    }

    private string GetCuriosityCountText()
    {
        return $"{Name} is used as a curiosity in {Curiosities.Count} rooms.";
    }
}
