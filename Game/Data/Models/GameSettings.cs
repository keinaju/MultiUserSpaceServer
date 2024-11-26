using System.ComponentModel.DataAnnotations;

namespace MUS.Game.Data.Models;

public class GameSettings
{
    [Key]
    public int PrimaryKey { get; set; }

    public Room DefaultSpawnRoom { get; set; }
}
