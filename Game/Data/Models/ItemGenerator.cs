using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class ItemGenerator
{
    [Key]
    public int PrimaryKey { get; set; }

    public Item Item { get; set; } = null!;

    public int MinQuantity { get; set; }
    public int MaxQuantity { get; set; }

    public int IntervalInTicks { get; set; }

    public ICollection<Inventory> Inventories { get; } = [];
}
