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
    _input.GetGroup(this.Regex, 1);

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IInputCommand _input;

    public RoomNameIsCommand(
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IInputCommand input
    )
    {
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            Respond();
            await RenameRoom();
        }
    }

    private async Task<bool> IsValid()
    {
        if(await _roomRepo.RoomNameIsReserved(NewNameInInput))
        {
            _response.AddText(
                Message.Reserved("room name", NewNameInInput)
            );
            return false;
        }

        return true;
    }

    private void Respond()
    {
        _response.AddText(
            Message.Renamed(CurrentRoom.Name, NewNameInInput)
        );
    }

    private async Task RenameRoom()
    {
        CurrentRoom.Name = NewNameInInput;

        await _roomRepo.UpdateRoom(CurrentRoom);
    }
}
