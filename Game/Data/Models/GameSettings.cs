using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MUS.Game.Data.Models;

public class GameSettings
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Being that is used to clone initial beings for new players.
    /// </summary>
    public int DefaultBeingPrimaryKey { get; set; }
    public required Being DefaultBeing
    {
        get => _lazyLoader.Load(this, ref _defaultBeing);
        set => _defaultBeing = value;
    }

    /// <summary>
    /// Room to spawn new beings.
    /// </summary>
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
    private Being _defaultBeing;
    private Room _defaultSpawnRoom;

    public GameSettings() {}

    private GameSettings(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
}
