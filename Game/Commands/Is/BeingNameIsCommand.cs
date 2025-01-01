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
    _input.GetGroup(this.Regex, 1);

    private Being CurrentBeing => _player.GetSelectedBeing();

    private readonly IBeingRepository _beingRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public BeingNameIsCommand(
        IBeingRepository beingRepo,
        IPlayerState player,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _beingRepo = beingRepo;
        _player = player;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            Respond();
            await RenameBeing();
        }
    }

    private async Task<bool> IsValid()
    {
        if(await _beingRepo
        .BeingNameIsReserved(NewNameInInput))
        {
            _response.AddText(
                Message.Reserved("being name", NewNameInInput)
            );
            return false;
        }

        return true;
    }

    private void Respond()
    {
        _response.AddText(
            Message.Renamed(CurrentBeing.Name, NewNameInInput)
        );
    }

    private async Task RenameBeing()
    {
        CurrentBeing.Name = NewNameInInput;

        await _beingRepo.UpdateBeing(CurrentBeing);
    }
}
