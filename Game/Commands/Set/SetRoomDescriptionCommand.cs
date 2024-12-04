using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetRoomDescriptionCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasPickedBeing
    ];
    
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;
    private string RoomDescription => GetParameter(1);

    protected override string Description =>
        "Sets a description for a room.";

    public SetRoomDescriptionCommand(
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^set room description (.+)$")
    {
        _state = state;
        _roomRepository = roomRepository;
    }

    public override async Task<string> Invoke()
    {
        var room = await _state.GetRoom();
        room.Description = RoomDescription;
        await _roomRepository.UpdateRoom(room);

        return MessageStandard.Set($"{room.Name}'s description", RoomDescription);
    }
}
