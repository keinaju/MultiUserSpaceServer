using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomNameIsCommand : IGameCommand
{
    public string HelpText => "Renames the current room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^room name is (.+)$");

    private string NewNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IUserInput _userInput;

    public RoomNameIsCommand(
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IUserInput userInput
    )
    {
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var responseText =
        Message.Renamed(CurrentRoom.Name, NewNameInInput);

        await SetRoomName();

        _response.AddText(
            responseText
        );
    }

    private async Task SetRoomName()
    {
        CurrentRoom.Name = NewNameInInput;

        await _roomRepo.UpdateRoom(CurrentRoom);
    }
}