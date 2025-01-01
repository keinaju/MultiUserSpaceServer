using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Delete;

public class DeleteRoomPoolCommand : IGameCommand
{
    public string HelpText => "Deletes a room pool.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^delete pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IInputCommand _input;

    public DeleteRoomPoolCommand(
        IResponsePayload response,
        IRoomRepository roomRepo,
        IRoomPoolRepository roomPoolRepo,
        IInputCommand input
    )
    {
        _response = response;
        _roomRepo = roomRepo;
        _roomPoolRepo = roomPoolRepo;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await DeleteCuriosities();
            await DeleteRoomPool();
            Respond();
        }
    }

    private async Task<bool> IsValid()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(RoomPoolNameInInput);
        if(roomPool is null)
        {
            _response.AddText(
                Message.DoesNotExist("room pool", RoomPoolNameInInput)
            );
            return false;
        }
        
        return true;
    }

    private async Task DeleteCuriosities()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(RoomPoolNameInInput);

        await _roomRepo.DeleteCuriosities(roomPool!);
    }

    private async Task DeleteRoomPool()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(RoomPoolNameInInput);

        await _roomPoolRepo
        .DeleteRoomPool(roomPool!.PrimaryKey);
    }

    private void Respond()
    {
        _response.AddText(
            Message.Deleted("room pool", RoomPoolNameInInput)
        );
    }
}
