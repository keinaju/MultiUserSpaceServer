using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Describe;

public class DescribeRoomCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];
    
    private readonly IRoomRepository _roomRepository;
    private string RoomName => GetParameter(1);
    private string RoomDescription => GetParameter(2);

    public DescribeRoomCommand(IRoomRepository roomRepository)
    : base(regex: @"^describe room (.+):(.+)$")
    {
        _roomRepository = roomRepository;
    }

    public override async Task<string> Invoke()
    {
        var room = await _roomRepository.FindRoom(RoomName);
        if(room is null)
        {
            return MessageStandard.DoesNotExist(RoomName);
        }

        room.Description = RoomDescription;
        await _roomRepository.UpdateRoom(room);

        return MessageStandard.Described(RoomName, RoomDescription);
    }
}
