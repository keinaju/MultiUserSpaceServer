using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomIsInRoomPoolCommand : IGameCommand
{
    public string HelpText =>
    "Adds the current room in a room pool.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^room is in pool (.+)$");

    private string RoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IInputCommand _input;

    public RoomIsInRoomPoolCommand(
        IPlayerState player,
        IResponsePayload response,
        IRoomPoolRepository roomPoolRepo,
        IInputCommand input
    )
    {
        _player = player;
        _response = response;
        _roomPoolRepo = roomPoolRepo;
        _input = input;
    }

    public async Task Run()
    {
        var roomPool =
        await _roomPoolRepo.FindRoomPool(
            RoomPoolNameInInput
        );
        if(roomPool is null)
        {
            _response.AddText(
                Message.DoesNotExist(
                    "room pool",
                    RoomPoolNameInInput
                )
            );

            return;
        }

        if(roomPool.HasRoom(CurrentRoom))
        {
            _response.AddText(
                $"{CurrentRoom.Name} is already in {roomPool.Name}."
            );
            
            return;
        }

        await AddRoomInRoomPool(roomPool);

        _response.AddText(
            $"{CurrentRoom.Name} was added to room pool {roomPool.Name}."
        );
    }

    private async Task AddRoomInRoomPool(RoomPool roomPool)
    {
        roomPool.Prototypes.Add(CurrentRoom);

        await _roomPoolRepo.UpdateRoomPool(roomPool);
    }
}
