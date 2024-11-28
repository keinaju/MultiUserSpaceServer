using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Rename;

public class RenameRoomCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    private readonly IRoomRepository _roomRepository;
    private string OldRoomName => GetParameter(1);
    private string NewRoomName => GetParameter(2);

    public RenameRoomCommand(IRoomRepository roomRepository)
    : base(regex: @"^rename room (.+):(.+)$")
    {
        _roomRepository = roomRepository;
    }

    public override async Task<string> Invoke()
    {
        var room = await _roomRepository.FindRoom(OldRoomName);
        if(room is null)
        {
            return MessageStandard.DoesNotExist(OldRoomName);
        }

        room.Name = NewRoomName;
        await _roomRepository.UpdateRoom(room);

        return MessageStandard.Renamed(OldRoomName, NewRoomName);
    }
}
