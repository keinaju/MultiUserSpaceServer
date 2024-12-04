﻿using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewRoomCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasPickedBeing,
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    public NewRoomCommand(
        IBeingRepository beingRepository,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^new room$")
    {
        _beingRepository = beingRepository;
        _state = state;
        _roomRepository = roomRepository;
    }

    public override async Task<string> Invoke()
    {
        var pickedBeing = await _state.GetBeing();

        var oldRoom = await _roomRepository.FindRoom(
            pickedBeing.InRoom.PrimaryKey
        );

        // Initialize an empty room
        var newRoom = new Room()
        {
            GlobalAccess = false,
            Name = string.Empty,
            Inventory = new Inventory()
        };

        //Connect new room to room it was created from
        newRoom.ConnectedToRooms.Add(oldRoom);
        newRoom = await _roomRepository.CreateRoom(newRoom);
        newRoom.Name = $"r{newRoom.PrimaryKey}";
        await _roomRepository.UpdateRoom(newRoom);

        //Connect old room to new room
        oldRoom.ConnectedToRooms.Add(newRoom);
        await _roomRepository.UpdateRoom(oldRoom);

        //Move being to new room
        pickedBeing.InRoom = newRoom;
        await _beingRepository.UpdateBeing(pickedBeing);

        return $"{MessageStandard.Created("room", newRoom.Name)} "
            + $"{pickedBeing.Name} moved there.";
    }
}
