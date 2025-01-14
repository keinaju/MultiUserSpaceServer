using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data.Models;

[Index(nameof(Name))]
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

    public async Task<CommandResult> AddRoom(Room room)
    {
        if(this.HasRoom(room))
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"Room pool {this.Name} already has the room {room.Name}."
            );
        }
        else
        {
            Prototypes.Add(room);

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Added("room", room.Name, $"{this.Name}'s prototypes")
            );
        }
    }

    public async Task<CommandResult> CreateExpansion(Room from, Being being)
    {
        if(Prototypes.Count == 0)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.DoesNotHave(Name, "prototypes"));
        }
        else
        {
            if(FeeItem is not null)
            {
                if(being.HasItems(1, FeeItem))
                {
                    await being.RemoveItems(1, FeeItem);
                }
                else
                {
                    return new CommandResult(StatusCode.Fail)
                    .AddMessage(
                        Message.DoesNotHave(
                            being.Name,
                            Message.Quantity(FeeItem.Name, 1)
                        )
                    );
                }
            }

            var expansion = await GenerateExpansion(from);

            return new CommandResult(StatusCode.Success)
            .AddMessage($"{expansion.Name} has been found.");
        }
    }

    public bool HasRoom(Room room)
    {
        return Prototypes.Contains(room);
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
    
    public string GetCuriosityPresentationText()
    {
        var sentences = new List<string>();

        if(this.Description is not null)
        {
            sentences.Add(this.Description);
        }

        if(this.FeeItem is null)
        {
            sentences.Add($"Exploring this is free.");
        }
        else
        {
            sentences.Add($"Exploring this costs {Message.Quantity(this.FeeItem.Name, 1)}.");
        }

        return string.Join(" ", sentences);
    }

    public async Task<CommandResult> SetDescription(string poolDescription)
    {
        var validationResult = TextSanitation.ValidateDescription(poolDescription);

        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        else
        {
            var cleanDescription = TextSanitation.GetCleanDescription(poolDescription);

            this.Description = cleanDescription;

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Set($"{this.Name}'s description", cleanDescription)
            );
        }
    }

    public async Task<CommandResult> SetFeeItem(Item item)
    {
        this.FeeItem = item;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set($"{Name}'s fee item", item.Name)
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
            if(await _context.RoomPoolNameIsReserved(cleanName))
            {
                return NameIsReserved("room pool", cleanName);
            }
            else
            {
                var message = Message.Renamed(this.Name, cleanName);

                this.Name = cleanName;

                await _context.SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage(message);
            }
        }
    }
    
    public CommandResult Show()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessages(this.GetDetails());
    }

    private async Task<Room> GenerateExpansion(Room from)
    {
        int randomIndex = new Random().Next(0, Prototypes.Count);
        // var randomPrototype = Prototypes.ToArray()[randomIndex];
        var randomPrototype = Prototypes.ElementAt(randomIndex);
        var expansion = randomPrototype.Clone();

        await _context.Rooms.AddAsync(expansion);

        expansion.Name = await _context.GetUniqueRoomName(expansion.Name);
        expansion.ConnectBidirectionally(from);

        await _context.SaveChangesAsync();

        return expansion;
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
