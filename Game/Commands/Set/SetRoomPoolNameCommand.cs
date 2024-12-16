using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomPoolNameCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    private readonly IGameResponse _response;
    private readonly IRoomPoolRepository _roomPoolRepository;
    private string OldRoomPoolName => GetParameter(1);
    private string NewRoomPoolName => GetParameter(2);

    protected override string Description =>
        "Sets a name for a room pool.";

    public SetRoomPoolNameCommand(
        IGameResponse response,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^set room pool (.+) name (.+)$")
    {
        _response = response;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task Invoke()
    {
        var roomPool = await _roomPoolRepository.FindRoomPool(OldRoomPoolName);
        if(roomPool is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist(
                    "Room pool", OldRoomPoolName
                )
            );
            return;
        }

        roomPool.Name = NewRoomPoolName;
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        _response.AddText(
            MessageStandard.Renamed(
                OldRoomPoolName, NewRoomPoolName
            )
        );
    }
}
