using System;
using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Deployment
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Item that is to be converted to being.
    /// </summary>
    public required Item Item { get; set; }

    /// <summary>
    /// Being to use for cloning.
    /// </summary>
    public required Being Prototype { get; set; }
}
