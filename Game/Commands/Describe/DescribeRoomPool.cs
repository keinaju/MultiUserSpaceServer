
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Describe;

public class DescribeRoomPool : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn
    ];

    private readonly IRoomPoolRepository _roomPoolRepository;
    private string RoomPoolName => GetParameter(1);
    private string RoomPoolDescription => GetParameter(2);

    public DescribeRoomPool(IRoomPoolRepository roomPoolRepository)
    : base(regex: @"^describe roompool (.+):(.+)$")
    {
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task<string> Invoke()
    {
        var roomPool = await _roomPoolRepository.FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            return MessageStandard.DoesNotExist(RoomPoolName);
        }

        roomPool.Description = RoomPoolDescription;
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        return MessageStandard.Described(RoomPoolName, RoomPoolDescription);
    }
}
