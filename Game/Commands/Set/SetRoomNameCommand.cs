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

    private readonly IGameResponse _response;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;
    private string RoomName => GetParameter(1);

    protected override string Description =>
    "Sets a name for the current room.";

    public SetRoomNameCommand(
        IGameResponse response,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^set room name (.+)$")
    {
        _response = response;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task Invoke()
    {
        var room = await _state.GetRoom();

        var oldName = room.Name;

        room.Name = RoomName;
        await _roomRepository.UpdateRoom(room);

        _response.AddText(
            MessageStandard.Renamed(oldName, RoomName)
        );
    }
}
