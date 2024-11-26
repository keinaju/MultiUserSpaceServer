using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class TickCounter
{
    [Key]
    public int PrimaryKey { get; set; }

    public ulong TickCount { get; set; }
}
