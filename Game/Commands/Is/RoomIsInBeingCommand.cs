using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomIsInBeingCommand : IGameCommand
{
    public string HelpText =>
    "Sets the current room as an inside room of the current being.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^room is inside$");

    private Being SelectedBeing => _player.GetSelectedBeing();

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IBeingRepository _beingRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;

    public RoomIsInBeingCommand(
        IBeingRepository beingRepo,
        IPlayerState player,
        IResponsePayload response
    )
    {
        _beingRepo = beingRepo;
        _player = player;
        _response = response;
    }

    public async Task Run()
    {
        SelectedBeing.RoomInside = CurrentRoom;
        await _beingRepo.UpdateBeing(SelectedBeing);

        _response.AddText(
            Message.Set(
                $"{SelectedBeing.Name}'s inside room",
                CurrentRoom.Name
            )
        );
    }
}
