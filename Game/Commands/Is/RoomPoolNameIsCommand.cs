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

    public Regex Regex => new("^pool name (.+) is (.+)$");

    private string OldRoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private string NewRoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IInputCommand _input;

    public RoomPoolNameIsCommand(
        IResponsePayload response,
        IRoomPoolRepository roomPoolRepo,
        IInputCommand input
    )
    {
        _response = response;
        _roomPoolRepo = roomPoolRepo;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await RenameRoomPool();
            Respond();
        }
    }

    private async Task<bool> IsValid()
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

            return false;
        }

        if(await _roomPoolRepo.RoomPoolNameIsReserved(
            NewRoomPoolNameInInput
        ))
        {
            _response.AddText(
                Message.Reserved(
                    "room pool name",
                    NewRoomPoolNameInInput
                )
            );

            return false;
        }

        return true;
    }

    private async Task RenameRoomPool()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(OldRoomPoolNameInInput);

        roomPool!.Name = NewRoomPoolNameInInput;

        await _roomPoolRepo.UpdateRoomPool(roomPool);
    }

    private void Respond()
    {
        _response.AddText(
            Message.Renamed(
                OldRoomPoolNameInInput,
                NewRoomPoolNameInInput
            )
        );
    }
}
