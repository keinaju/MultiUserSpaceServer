using MUS.Game.Session;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewBeingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn
    ];

    protected override string Description =>
    "Creates a new being.";

    private readonly IBeingRepository _beingRepository;
    private readonly IGameResponse _response;
    private readonly IGameSettingsRepository _gameSettingsRepository;
    private readonly ISessionService _session;

    public NewBeingCommand(
        IBeingRepository beingRepository,
        IGameResponse response,
        IGameSettingsRepository gameSettingsRepository,
        ISessionService session
    )
    : base(regex: @"^new being$")
    {
        _beingRepository = beingRepository;
        _gameSettingsRepository = gameSettingsRepository;
        _response = response;
        _session = session;
    }

    public override async Task Invoke()
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
        
        _response.AddText(
            MessageStandard.Created(
                "being", savedBeing.Name
            )
        );
    }
}
