using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowRoomPoolsCommand : IGameCommand
{
    public string HelpText => "Shows all room pools.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^show room pools$");

    private readonly IResponsePayload _response;
    private readonly IRoomPoolRepository _ropoRepo;

    public ShowRoomPoolsCommand(
        IResponsePayload response,
        IRoomPoolRepository ropoRepo
    )
    {
        _response = response;
        _ropoRepo = ropoRepo;
    }

    public async Task Run()
    {
        var roomPools = await _ropoRepo.FindRoomPools();
        if(roomPools.Count == 0)
        {
            _response.AddText("There are no room pools.");
            return;
        }

        _response.AddText("Room pools are:");

        _response.AddText(GetRoomPoolNames(roomPools));
    }

    private string GetRoomPoolNames(IEnumerable<RoomPool> roomPools)
    {
        var roomPoolNames = new List<string>();

        foreach(var pool in roomPools)
        {
            roomPoolNames.Add(pool.Name);
        }

        roomPoolNames.Sort();

        return Message.List(roomPoolNames);
    }
}
