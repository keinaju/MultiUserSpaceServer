using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class NewCuriosityCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IRoomPoolRepository _roomPoolRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerState _state;

    private string RoomPoolName => GetParameter(1);

    public NewCuriosityCommand(
        IRoomPoolRepository roomPoolRepository,
        IRoomRepository roomRepository,
        IPlayerState state
    )
    : base(regex: @"^new (.+) curiosity$")
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
        if(currentRoom.Curiosity is not null)
        {
            return $"{currentRoom.Name} already has an curiosity.";
        }
        
        currentRoom.Curiosity = new Curiosity() {
            RoomPool = roomPool
        };
        await _roomRepository.UpdateRoom(currentRoom);

        return $"Curiosity was created in {currentRoom.Name}.";
    }
}
