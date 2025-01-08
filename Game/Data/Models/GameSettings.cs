using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MUS.Game.Data.Models;

public class GameSettings
{
    [Key]
    public int PrimaryKey { get; set; }

    public int DefaultSpawnRoomPrimaryKey { get; set; }
    public required Room DefaultSpawnRoom
    {
        get => _lazyLoader.Load(this, ref _defaultSpawnRoom);
        set => _defaultSpawnRoom = value;
    }

    public required string GameName { get; set; }

    public required string? GameDescription { get; set; }

    public required int TickIntervalSeconds { get; set; }

    private readonly ILazyLoader _lazyLoader;
    private Room _defaultSpawnRoom;

    public GameSettings() {}

    private GameSettings(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
}
