using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowGlobalRoomsCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IRoomRepository _roomRepository;

    protected override string Description =>
        "Shows all rooms that can be accessed from anywhere.";
    
    public ShowGlobalRoomsCommand(IRoomRepository roomRepository)
    : base(regex: @"^show global rooms$")
    {
        _roomRepository = roomRepository;
    }

    public override async Task<string> Invoke()
    {
        var rooms = await _roomRepository.FindGlobalRooms();
        if(rooms.Count == 0)
        {
            return "There are no global rooms.";
        }

        var roomNames = new List<string>();
        foreach(var room in rooms)
        {
            roomNames.Add(room.Name);
        }

        return $"Global rooms are: {MessageStandard.List(roomNames)}.";
    }
}
