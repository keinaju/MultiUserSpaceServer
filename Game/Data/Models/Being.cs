using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Being
{
    [Key]
    public int PrimaryKey { get; set; }

    public int CreatedByUserPrimaryKey { get; set; }
    public required User CreatedByUser { get; set; } = null!;

    /// <summary>
    /// Features associated with this being.
    /// </summary>
    public ICollection<Feature> Features { get; } = new HashSet<Feature>();

    /// <summary>
    /// Location of being.
    /// </summary>
    public required Room InRoom { get; set; }

    public required Inventory Inventory { get; set; }

    public required string? Name { get; set; }

    /// <summary>
    /// Optional room inside being, e.g. vehicle room.
    /// </summary>
    public Room? RoomInside { get; set; }

    public Being Clone()
    {
        var clone = new Being()
        {
            CreatedByUser = this.CreatedByUser,
            InRoom = this.InRoom,
            Inventory = this.Inventory.Clone(),
            Name = this.Name,
            RoomInside = this.RoomInside is null ?
                null : this.RoomInside.Clone()
        };

        foreach(var feature in this.Features)
        {
            clone.Features.Add(feature);
        }

        return clone;
    }
}
