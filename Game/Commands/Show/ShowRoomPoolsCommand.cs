using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowRoomPoolsCommand : IGameCommand
{
    public string HelpText => "Shows all room pools.";

    public Regex Pattern => new("^(show|s) pools$");

    private readonly GameContext _context;

    public ShowRoomPoolsCommand(
        GameContext context
    )
    {
        _context = context;
    }

    public async Task<CommandResult> Run()
    {
        var pools = await _context.FindAllRoomPools();
        if(pools.Count == 0)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage("There are no room pools.");
        }
        else
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"All room pools are: {GetRoomPoolNames(pools)}.");
        }
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
