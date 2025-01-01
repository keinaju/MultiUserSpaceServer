using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Delete;

public class DeleteRoomCommand : IGameCommand
{
    public string HelpText => "Deletes a room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^delete room (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IInputCommand _input;

    public DeleteRoomCommand(
        IResponsePayload response,
        IRoomRepository roomRepo,
        IInputCommand input
    )
    {
        _response = response;
        _roomRepo = roomRepo;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await DeleteRoom();
            Respond();
        }
    }

    private async Task<bool> IsValid()
    {
        var room = await _roomRepo.FindRoom(RoomNameInInput);

        if(room is null)
        {
            _response.AddText(
                Message.DoesNotExist("room", RoomNameInInput)
            );
            return false;
        }

        if(room.BeingsHere.Count > 0)
        {
            _response.AddText(
                Message.Has(room.Name, "beings") +
                $" Move all beings from {room.Name} to other rooms before trying to delete the room."
            );
            return false;
        }

        return true;
    }

    private async Task DeleteRoom()
    {
        var room = await _roomRepo.FindRoom(RoomNameInInput);

        await _roomRepo.DeleteRoom(room!.PrimaryKey);
    }

    private void Respond()
    {
        _response.AddText(
            Message.Deleted("room", RoomNameInInput)
        );
    }
}
