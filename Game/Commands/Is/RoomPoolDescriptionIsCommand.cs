using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomPoolDescriptionIsCommand : IGameCommand
{
    public string HelpText =>
    "Sets the description of a room pool.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex =>
    new("^room pool (.+) description is (.+)$");

    private string RoomPoolNameInInput =>
    _userInput.GetGroup(this.Regex, 1);
    
    private string RoomPoolDescriptionInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IUserInput _userInput;

    public RoomPoolDescriptionIsCommand(
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
        .FindRoomPool(RoomPoolNameInInput);
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

        await SetRoomPoolDescription(roomPool);

        _response.AddText(
            Message.Set(
                $"{roomPool.Name}'s description",
                RoomPoolDescriptionInInput
            )
        );
    }

    private async Task SetRoomPoolDescription(RoomPool roomPool)
    {
        roomPool.Description = RoomPoolDescriptionInInput;

        await _roomPoolRepo.UpdateRoomPool(roomPool);
    }
}
