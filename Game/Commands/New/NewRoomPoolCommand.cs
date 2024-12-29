using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewRoomPoolCommand : IGameCommand
{
    public string HelpText => "Creates a new room pool.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^new room pool$");

    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _roomPoolRepo;

    public NewRoomPoolCommand(
        IResponsePayload response,
        IRoomPoolRepository roomPoolRepo
    )
    {
        _response = response;
        _roomPoolRepo = roomPoolRepo;
    }

    public async Task Run()
    {
        var roomPool = await CreateRoomPool();

        _response.AddText(
            Message.Created("room pool", roomPool.Name)
        );
    }

    private async Task<RoomPool> CreateRoomPool()
    {
        return await _roomPoolRepo.CreateRoomPool(
            new RoomPool()
            {
                Description = string.Empty,
                FeeItem = null,
                Name = "pool #"
            }
        );
    }
}
