using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class RoomDescriptionIsCommand : IGameCommand
{
    public string HelpText =>
    "Sets the description of the current room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^room description is (.+)$");

    private Room CurrentRoom => _player.GetCurrentRoom();

    private string DescriptionInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IUserInput _userInput;

    public RoomDescriptionIsCommand(
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
        await SetRoomDescription();

        _response.AddText(
            Message.Set(
                $"{CurrentRoom.Name}'s description",
                DescriptionInInput
            )
        );
    }

    private async Task SetRoomDescription()
    {
        CurrentRoom.Description = DescriptionInInput;

        await _roomRepo.UpdateRoom(CurrentRoom);
    }
}
