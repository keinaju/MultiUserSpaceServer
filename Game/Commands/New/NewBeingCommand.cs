using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewBeingCommand : IGameCommand
{
    public string HelpText => "Creates a new being.";

    public Regex Regex => new("^new being$");

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn
    ];

    private readonly IBeingRepository _beingRepo;
    private readonly IGameSettingsRepository _settingsRepo;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public NewBeingCommand(
        IBeingRepository beingRepo,
        IGameSettingsRepository settingsRepo,
        IResponsePayload response,
        ISessionService session
    )
    {
        _beingRepo = beingRepo;
        _settingsRepo = settingsRepo;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        var being = await CreateBeing();

        _response.AddText(
            Message.Created("being", being.Name)
        );
    }

    private async Task<Being> CreateBeing()
    {
        var being = new Being()
        {
            Name = await _beingRepo.GetUniqueBeingName("being #"),
            CreatedByUser = _session.AuthenticatedUser!,
            Inventory = new Inventory(),
            InRoom = await GetSpawnRoom()
        };

        return await _beingRepo.CreateBeing(being);
    }

    private async Task<Room> GetSpawnRoom()
    {
        var settings =
        await _settingsRepo.GetGameSettings();

        return settings!.DefaultSpawnRoom;
    }
}
