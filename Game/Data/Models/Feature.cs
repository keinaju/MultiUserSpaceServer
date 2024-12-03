using System;
using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class Feature
{
    [Key]
    public int PrimaryKey { get; set; }

    public required string Name { get; set; }

    public ICollection<Being> AssociatedWithBeings { get; }
        = new HashSet<Being>();

    public ICollection<Room> MustHaveInRooms { get; }
        = new HashSet<Room>();
}
