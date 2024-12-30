using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowGlobalRoomsCommand : IGameCommand
{
    public string HelpText => "Shows all globally accessible rooms.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(show|s) global rooms$");

    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;

    public ShowGlobalRoomsCommand(
        IResponsePayload response,
        IRoomRepository roomRepo
    )
    {
        _response = response;
        _roomRepo = roomRepo;
    }

    public async Task Run()
    {
        var rooms = await _roomRepo.FindGlobalRooms();

        if(rooms.Count == 0)
        {
            _response.AddText("There are no global rooms.");
            return;
        }

        _response.AddText($"All global rooms are: {GetRoomNames(rooms)}.");
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
