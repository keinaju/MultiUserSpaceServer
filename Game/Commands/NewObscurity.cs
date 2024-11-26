using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class NewObscurityCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IRoomPoolRepository _roomPoolRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerState _state;

    private string RoomPoolName => GetParameter(1);

    public NewObscurityCommand(
        IRoomPoolRepository roomPoolRepository,
        IRoomRepository roomRepository,
        IPlayerState state
    )
    : base(regex: @"^new (.*) obscurity$")
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
            return $"{RoomPoolName} does not exist.";
        }

        var currentRoom = await _state.CurrentRoom();
        if(currentRoom.Obscurity is not null)
        {
            return $"{currentRoom.Name} already has an obscurity.";
        }
        
        currentRoom.Obscurity = new Obscurity() {
            RoomPool = roomPool
        };
        await _roomRepository.UpdateRoom(currentRoom);

        return $"Obscurity was created in {currentRoom.Name}.";
    }
}
