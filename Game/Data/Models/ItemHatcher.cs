using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class ItemHatcher
{
    [Key]
    public int PrimaryKey { get; set; }

    public required Item Item { get; set; } = null!;

    public required int MinQuantity { get; set; }
    public required int MaxQuantity { get; set; }

    public required int IntervalInTicks { get; set; }

    public ICollection<Inventory> Inventories { get; } = new HashSet<Inventory>();
}
