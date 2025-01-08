using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowGlobalRoomsCommand : IGameCommand
{
    public string HelpText => "Shows all globally accessible rooms.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(show|s) global rooms$");

    private readonly GameContext _context;
    private readonly IResponsePayload _response;

    public ShowGlobalRoomsCommand(
        GameContext context,
        IResponsePayload response
    )
    {
        _context = context;
        _response = response;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ShowGlobalRooms()
        );
    }

    private async Task<CommandResult> ShowGlobalRooms()
    {
        var globalRooms = await _context.FindAllGlobalRooms();

        if(globalRooms.Count == 0)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage("There are no global rooms.");
        }
        else
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"All global rooms are: {GetRoomNames(globalRooms)}.");
        }
    }

    private string GetRoomNames(IEnumerable<Room> rooms)
    {
        var roomNames = new List<string>();

        foreach(var room in rooms)
        {
            roomNames.Add(room.Name);
        }

        roomNames.Sort();

        return Message.List(roomNames);
    }
}
