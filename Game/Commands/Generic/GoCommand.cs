using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class GoCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public string HelpText =>
    "Moves a selected being to another room.";

    public Regex Regex => new("^go (.+)$");

    private string RoomNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private Being SelectedBeing => _player.GetSelectedBeing();

    private Room CurrentRoom => _player.GetCurrentRoom();

    private readonly IBeingRepository _beingRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IInputCommand _input;

    public GoCommand(
        IBeingRepository beingRepo,
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IInputCommand input
    )
    {
        _beingRepo = beingRepo;
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
        _input = input;
    }

    public async Task Run()
    {
        var destination = await _roomRepo.FindRoom(RoomNameInInput);
        
        if(destination is null)
        {
            _response.AddText(
                Message.DoesNotExist("room", RoomNameInInput)
            );
            return;
        }

        if(destination == CurrentRoom)
        {
            _response.AddText(
                $"{SelectedBeing.Name} is in {destination.Name}."
            );
            return;
        }

        if(!CurrentRoom.HasAccessTo(destination))
        {
            _response.AddText(
                $"{destination.Name} can not be accessed "
                + $"from {CurrentRoom.Name}."
            );
            return;
        }

        if(destination == SelectedBeing.RoomInside)
        {
            _response.AddText(
                $"{SelectedBeing.Name} can not enter itself."
            );
            return;
        }

        await MoveSelectedBeing(destination);

        _response.AddText(
            $"{SelectedBeing.Name} moved to {destination.Name}."
        );
    }

    private async Task MoveSelectedBeing(Room destination)
    {
        SelectedBeing.InRoom = destination;

        await _beingRepo.UpdateBeing(SelectedBeing);
    }
}
