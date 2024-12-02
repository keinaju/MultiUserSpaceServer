using System;
using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class CraftComponent
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Quantity of items needed to complete a product of craft plan.
    /// </summary>
    public required int Quantity { get; set; }

    /// <summary>
    /// Item needed to complete a product of craft plan.
    /// </summary>
    public required Item Item { get; set; }
}
