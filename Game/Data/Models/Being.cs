using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Being
{
    [Key]
    public int PrimaryKey { get; set; }

    public int CreatedByUserPrimaryKey { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public required Inventory Inventory { get; set; }

    public required string? Name { get; set; }

    /// <summary>
    /// Location of being.
    /// </summary>
    public required Room InRoom { get; set; }

    /// <summary>
    /// Optional room inside being, e.g. vehicle room.
    /// </summary>
    public Room? RoomInside { get; set; }
}
