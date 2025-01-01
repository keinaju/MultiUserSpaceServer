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
    _input.GetGroup(this.Regex, 1);

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IInputCommand _input;

    public RoomDescriptionIsCommand(
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
