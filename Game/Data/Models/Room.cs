using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUS.Game.Data.Models;

public class Room
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Setting to determine if room can be accessed from any other room.
    /// </summary>
    public required bool GlobalAccess { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public ICollection<Room> ConnectedToRooms { get; } = new HashSet<Room>();
    public ICollection<Room> ConnectedFromRooms { get; } = new HashSet<Room>();

    public Inventory Inventory { get; set; } = null!;

    public RoomPool? Curiosity { get; set; }

    [InverseProperty(nameof(Being.InRoom))]
    public ICollection<Being> BeingsHere { get; } = new HashSet<Being>();

    public Room Clone()
    {
        var clone = new Room()
        {
            Curiosity = this.Curiosity,
            Description = this.Description,
            GlobalAccess = this.GlobalAccess,
            Inventory = this.Inventory.Clone(),
            Name = this.Name,
        };

        return clone;
    }
}
