
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomPoolDescriptionCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    private readonly IGameResponse _response;
    private readonly IRoomPoolRepository _roomPoolRepository;
    private string RoomPoolName => GetParameter(1);
    private string RoomPoolDescription => GetParameter(2);

    protected override string Description =>
    "Sets a description for a room pool.";

    public SetRoomPoolDescriptionCommand(
        IGameResponse response,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^set room pool (.+) description (.+)$")
    {
        _response = response;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task Invoke()
    {
        var roomPool = await _roomPoolRepository
        .FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist(
                    "Room pool", RoomPoolName
                )
            );
            return;
        }

        roomPool.Description = RoomPoolDescription;
        await _roomPoolRepository.UpdateRoomPool(roomPool);

        _response.AddText(
            MessageStandard.Set(
                $"{RoomPoolName}'s description",
                RoomPoolDescription
            )
        );
    }
}
