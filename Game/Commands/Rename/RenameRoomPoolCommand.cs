using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Rename;

public class RenameRoomPoolCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    private readonly IRoomPoolRepository _roomPoolRepository;
    private string OldRoomPoolName => GetParameter(1);
    private string NewRoomPoolName => GetParameter(2);

    public RenameRoomPoolCommand(
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^rename roompool (.+):(.+)$")
    {
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task<string> Invoke()
    {
        var roomPool = await _roomPoolRepository
            .FindRoomPool(OldRoomPoolName);
        if(roomPool is null)
        {
            return MessageStandard.DoesNotExist(OldRoomPoolName);
        }

        roomPool.Name = NewRoomPoolName;
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        return MessageStandard.Renamed(OldRoomPoolName, NewRoomPoolName);
    }
}
