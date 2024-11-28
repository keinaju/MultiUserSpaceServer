using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomNameCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;
    private string RoomName => GetParameter(1);

    public SetRoomNameCommand(
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^set room name (.+)$")
    {
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var room = await _state.Room();

        var oldName = room.Name;

        room.Name = RoomName;
        await _roomRepository.UpdateRoom(room);

        return MessageStandard.Renamed(oldName, RoomName);
    }
}
