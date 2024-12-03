using System;
using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Feature
{
    [Key]
    public int PrimaryKey { get; set; }

    public required string Name { get; set; }
}
