using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class RoomPool
{
    [Key]
    public int PrimaryKey { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public ICollection<RoomInPool> RoomsInPool { get; } = new HashSet<RoomInPool>();
}
