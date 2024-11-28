using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Curiosity
{
    [Key]
    public int PrimaryKey { get; set; }

    public string? Description { get; set; }

    public required RoomPool RoomPool { get; set; }

    public Curiosity Clone()
    {
        return new Curiosity()
        {
            Description = this.Description,
            RoomPool = this.RoomPool
        };
    }
}
