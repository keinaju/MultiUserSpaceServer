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

    private readonly IGameResponse _response;
    private readonly IRoomPoolRepository _roomPoolRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerState _state;

    private string RoomPoolName => GetParameter(1);

    protected override string Description =>
        "Sets a room pool to use to generate cloned rooms in the current room.";

    public SetCuriosityCommand(
        IGameResponse response,
        IRoomPoolRepository roomPoolRepository,
        IRoomRepository roomRepository,
        IPlayerState state
    )
    : base(regex: @"^set curiosity (.+)$")
    {
        _response = response;
        _roomPoolRepository = roomPoolRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task Invoke()
    {
        var roomPool = await _roomPoolRepository.FindRoomPool(RoomPoolName);
        if(roomPool is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Room pool", RoomPoolName)
            );
            return;
        }

        var currentRoom = await _state.GetRoom();
        currentRoom.Curiosity = roomPool;
        await _roomRepository.UpdateRoom(currentRoom);

        _response.AddText(
            MessageStandard.Set(
                $"{currentRoom.Name}'s curiosity",
                roomPool.Name
            )
        );
    }
}
