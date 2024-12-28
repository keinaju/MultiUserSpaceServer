using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomPoolNameIsCommand : IGameCommand
{
    public string HelpText => "Renames a room pool.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^room pool name (.+) is (.+)$");

    private string OldRoomPoolNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private string NewRoomPoolNameInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IUserInput _userInput;

    public RoomPoolNameIsCommand(
        IResponsePayload response,
        IRoomPoolRepository roomPoolRepo,
        IUserInput userInput
    )
    {
        _response = response;
        _roomPoolRepo = roomPoolRepo;
        _userInput = userInput;
    }

    public async Task Run()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(OldRoomPoolNameInInput);
        if(roomPool is null)
        {
            _response.AddText(
                Message.DoesNotExist(
                    "room pool", OldRoomPoolNameInInput
                )
            );

            return;
        }

        await SetRoomPoolName(roomPool);

        _response.AddText(
            Message.Renamed(
                OldRoomPoolNameInInput,
                NewRoomPoolNameInInput
            )
        );
    }

    private async Task SetRoomPoolName(RoomPool roomPool)
    {
        roomPool.Name = NewRoomPoolNameInInput;

        await _roomPoolRepo.UpdateRoomPool(roomPool);
    }
}
