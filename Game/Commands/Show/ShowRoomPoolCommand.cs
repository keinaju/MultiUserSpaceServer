using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowRoomPoolCommand : IGameCommand
{
    public string HelpText => "Shows details about a room pool.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^(show|s) pool (.+)$");

    private string RoomPoolNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _roomPoolRepo;
    private readonly IInputCommand _input;

    public ShowRoomPoolCommand(
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
            await ShowRoomPool();
        }
    }

    private async Task<bool> IsValid()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(RoomPoolNameInInput);
        
        if(roomPool is null)
        {
            _response.AddText(
                Message.DoesNotExist(
                    "room pool", RoomPoolNameInInput
                )
            );
            return false;
        }

        return true;
    }

    private async Task ShowRoomPool()
    {
        var roomPool = await _roomPoolRepo
        .FindRoomPool(RoomPoolNameInInput);

        _response.AddList(roomPool!.GetDetails());
    }
}
