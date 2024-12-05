using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetCuriosityCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IRoomPoolRepository _roomPoolRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerState _state;

    private string RoomPoolName => GetParameter(1);

    protected override string Description =>
        "Sets a room pool to use to generate cloned rooms in the current room.";

    public SetCuriosityCommand(
        IRoomPoolRepository roomPoolRepository,
        IRoomRepository roomRepository,
        IPlayerState state
    )
    : base(regex: @"^set curiosity (.+)$")
    {
        _roomPoolRepository = roomPoolRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var roomPool = await _roomPoolRepository.FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            return MessageStandard.DoesNotExist("Room pool", RoomPoolName);
        }

        var currentRoom = await _state.GetRoom();
        currentRoom.Curiosity = roomPool;
        await _roomRepository.UpdateRoom(currentRoom);

        return MessageStandard.Set(
            $"{currentRoom.Name}'s curiosity",
            roomPool.Name
        );
    }
}
