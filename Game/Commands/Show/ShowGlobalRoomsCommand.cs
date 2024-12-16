using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowGlobalRoomsCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IGameResponse _response;
    private readonly IRoomRepository _roomRepository;

    protected override string Description =>
        "Shows all rooms that can be accessed from anywhere.";
    
    public ShowGlobalRoomsCommand(
        IGameResponse response,
        IRoomRepository roomRepository
    )
    : base(regex: @"^show global rooms$")
    {
        _response = response;
        _roomRepository = roomRepository;
    }

    public override async Task Invoke()
    {
        var rooms = await _roomRepository.FindGlobalRooms();
        if(rooms.Count == 0)
        {
            _response.AddText("There are no global rooms.");
            return;
        }

        var roomNames = new List<string>();
        foreach(var room in rooms)
        {
            roomNames.Add(room.Name);
        }

        _response.AddText(
            $"Global rooms are: {MessageStandard.List(roomNames)}."
        );
    }
}
