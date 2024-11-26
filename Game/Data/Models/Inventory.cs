using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Inventory
{
    [Key]
    public int PrimaryKey { get; set; }

    public ICollection<ItemGenerator> ItemGenerators { get; } = [];
}
