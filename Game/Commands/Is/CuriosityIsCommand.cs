using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class CuriosityIsCommand : IGameCommand
{
    public string HelpText => "Sets the curiosity in the current room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^curiosity is (.+)$");

    private Room CurrentRoom => _player.GetCurrentRoom();

    private string RoomPoolNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IUserInput _userInput;

    public CuriosityIsCommand(
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IRoomPoolRepository roomPoolRepo,
        IUserInput userInput
    )
    {
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
        _roomPoolRepo = roomPoolRepo;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(RoomPoolNameInInput);
        if(roomPool is null)
        {
            _response.AddText(
                Message.DoesNotExist(
                    "room pool", RoomPoolNameInInput
                )
            );
            return;
        }

        await SetCuriosity(roomPool);

        _response.AddText(
            Message.Set(
                $"{CurrentRoom.Name}'s curiosity",
                roomPool.Name
            )
        );
    }

    private async Task SetCuriosity(RoomPool roomPool)
    {
        CurrentRoom.Curiosity = roomPool;

        await _roomRepo.UpdateRoom(CurrentRoom);
    }
}
