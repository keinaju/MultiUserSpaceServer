using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
    private Item? _feeItem;
    private ICollection<Room> _prototypes;
    private ICollection<Room> _curiosities;

    public RoomPool() {}

    private RoomPool(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    public Room CreateExtensionRoom()
    {
        int random = new Random().Next(0, Prototypes.Count);

        var randomPrototype = Prototypes.ToArray()[random];

        return randomPrototype.Clone();
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
}
