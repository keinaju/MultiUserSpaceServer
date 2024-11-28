using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Item
{
    [Key]
    public int PrimaryKey { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
}
