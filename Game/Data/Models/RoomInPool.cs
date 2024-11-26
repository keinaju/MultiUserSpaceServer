using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class RoomInPool
{
    [Key]
    public int PrimaryKey { get; set; }

    public Room Room { get; set; } = null!;
}
