using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Room
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Setting to determine if room can be accessed from any other room.
    /// </summary>
    public required bool GlobalAccessibility { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public ICollection<Room> ConnectedToRooms { get; } = new HashSet<Room>();
    public ICollection<Room> ConnectedFromRooms { get; } = new HashSet<Room>();

    public Inventory Inventory { get; set; } = null!;

    public RoomPool? Curiosity { get; set; }

    public Room Clone()
    {
        var clone = new Room()
        {
            Curiosity = this.Curiosity,
            Description = this.Description,
            GlobalAccessibility = this.GlobalAccessibility,
            Inventory = new Inventory(),
            Name = this.Name,
        };

        return clone;
    }
}
