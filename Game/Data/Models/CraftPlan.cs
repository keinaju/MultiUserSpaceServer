using System;
using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class CraftPlan
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Item to produce if all components are present.
    /// </summary>
    public required Item Product { get; set; }

    /// <summary>
    /// Collection of items and quantities needed to produce item of craft plan.
    /// </summary>
    public ICollection<CraftComponent> Components { get; }
        = new HashSet<CraftComponent>();
}
