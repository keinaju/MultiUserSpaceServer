using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomNameCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;
    private string RoomName => GetParameter(1);

    protected override string Description =>
        "Sets a name for the current room.";

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
        var room = await _state.GetRoom();

        var oldName = room.Name;

        room.Name = RoomName;
        await _roomRepository.UpdateRoom(room);

        return MessageStandard.Renamed(oldName, RoomName);
    }
}
