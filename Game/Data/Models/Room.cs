using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Room
{
    [Key]
    public int PrimaryKey { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public ICollection<Room> ConnectedToRooms { get; } = new HashSet<Room>();
    public ICollection<Room> ConnectedFromRooms { get; } = new HashSet<Room>();

    public Inventory Inventory { get; set; } = null!;

    public Curiosity? Curiosity { get; set; }

    public Room Clone()
    {
        var clone = new Room()
        {
            Name = this.Name,
            Description = this.Description,
            Inventory = new Inventory(),
            Curiosity = this.Curiosity is null ? null : this.Curiosity.Clone()
        };

        return clone;
    }
}
