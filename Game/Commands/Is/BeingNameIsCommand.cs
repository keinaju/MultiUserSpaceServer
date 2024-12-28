using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class BeingNameIsCommand : IGameCommand
{
    public string HelpText =>
    "Renames the currently selected being.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^being name is (.+)$");

    private string NewNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private Being CurrentBeing => _player.GetSelectedBeing();

    private readonly IBeingRepository _beingRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public BeingNameIsCommand(
        IBeingRepository beingRepo,
        IPlayerState player,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _beingRepo = beingRepo;
        _player = player;
        _response = response;
        _userInput = userInput;
    }

    public async Task Run()
    {
        string responseText =
        Message.Renamed(CurrentBeing.Name, NewNameInInput);

        await SetBeingName();

        _response.AddText(responseText);
    }

    private async Task SetBeingName()
    {
        CurrentBeing.Name = NewNameInInput;

        await _beingRepo.UpdateBeing(CurrentBeing);
    }
}
