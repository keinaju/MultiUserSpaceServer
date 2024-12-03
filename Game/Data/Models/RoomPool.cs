using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class RoomPool
{
    [Key]
    public int PrimaryKey { get; set; }

    public required string Description { get; set; }

    /// <summary>
    /// Item that is needed to explore this room pool.
    /// Null if no item is needed to explore.
    /// </summary>
    public required Item? ItemToExplore { get; set; }

    public required string Name { get; set; }

    public ICollection<RoomInPool> RoomsInPool { get; } = new HashSet<RoomInPool>();
}
