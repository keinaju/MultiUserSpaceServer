using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Being
{
    [Key]
    public int PrimaryKey { get; set; }

    public int CreatedByUserPrimaryKey { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public Inventory Inventory { get; set; } = null!;

    public string? Name { get; set; }

    public Room Room { get; set; } = null!;
}
