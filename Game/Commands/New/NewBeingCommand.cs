using MUS.Game.Session;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewBeingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IGameSettingsRepository _gameSettingsRepository;
    private readonly ISessionService _session;

    public NewBeingCommand(
        IBeingRepository beingRepository,
        IGameSettingsRepository gameSettingsRepository,
        ISessionService session
    )
    : base(regex: @"^new being$")
    {
        _beingRepository = beingRepository;
        _gameSettingsRepository = gameSettingsRepository;
        _session = session;
    }

    public override async Task<string> Invoke()
    {
        var user = _session.AuthenticatedUser!;
        var settings = await _gameSettingsRepository.GetGameSettings();
        var defaultSpawnRoom = settings!.DefaultSpawnRoom;
        var newBeing = new Being()
        {
            Name = string.Empty,
            CreatedByUser = user,
            Inventory = new Inventory(),
            InRoom = defaultSpawnRoom
        };
        var savedBeing = await _beingRepository.CreateBeing(newBeing);
        savedBeing.Name = $"b{savedBeing.PrimaryKey}";
        await _beingRepository.UpdateBeing(savedBeing);
        return MessageStandard.Created("being", savedBeing.Name);
    }
}
