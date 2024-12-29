using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewRoomCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing,
        Condition.UserIsBuilder
    ];

    public string HelpText =>
    "Creates a new room and connects it to the current room.";

    public Regex Regex => new("^new room$");

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;

    public NewRoomCommand(
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo
    )
    {
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
    }

    public async Task Run()
    {
        var newRoom = await CreateRoom();

        await SetBidirectionalConnection(newRoom, CurrentRoom);

        _response.AddText(
            Message.Created("room", newRoom.Name)
        );
    }

    private async Task<Room> CreateRoom()
    {
        return await _roomRepo.CreateRoom(
            new Room()
            {
                GlobalAccess = false,
                Name = "room #",
                Inventory = new Inventory(),
                InBeing = null
            }
        );
    }

    private async Task SetBidirectionalConnection(
        Room from, Room to
    )
    {
        from.ConnectBidirectionally(to);

        await _roomRepo.UpdateRoom(from);
        await _roomRepo.UpdateRoom(to);
    }
}
