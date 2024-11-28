using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Inventory
{
    [Key]
    public int PrimaryKey { get; set; }

    public ICollection<ItemStack> ItemStacks { get; set; } = new HashSet<ItemStack>();

    public ICollection<ItemGenerator> ItemGenerators { get; } = [];

    public bool IsEmpty => ItemStacks.Count == 0;
}
