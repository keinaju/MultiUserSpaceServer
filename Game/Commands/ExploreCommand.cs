using MUS.Game.Data;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class ExploreCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IPlayerState _state;
    private readonly ICuriosityRepository _curiosityRepository;
    private readonly IRoomRepository _roomRepository;

    public ExploreCommand(
        IPlayerState state,
        ICuriosityRepository curiosityRepository,
        IRoomRepository roomRepository
    )
    : base(regex: @"^explore$")
    {
        _curiosityRepository = curiosityRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var currentRoom = await _state.CurrentRoom();

        var curiosity = currentRoom.Curiosity;
        if(curiosity is null)
        {
            return $"{currentRoom.Name} does not have a curiosity to explore.";
        }
        curiosity = await _curiosityRepository
            .FindCuriosity(curiosity.PrimaryKey);

        var roomsInPool = curiosity.RoomPool.RoomsInPool;
        if(roomsInPool.Count == 0)
        {
            return $"Room pool {curiosity.RoomPool.Name} is empty.";
        }

        int random = new Random().Next(0, roomsInPool.Count);

        var randomRoom = roomsInPool.ToArray()[random].Room;
        // Populate curiosity
        randomRoom = await _roomRepository.FindRoom(randomRoom.PrimaryKey);

        var clone = randomRoom.Clone();
        clone = await _roomRepository.CreateRoom(clone);
        clone.Name = $"{clone.Name}-{clone.PrimaryKey}";

        clone.ConnectedToRooms.Add(currentRoom);
        await _roomRepository.UpdateRoom(clone);
        currentRoom.ConnectedToRooms.Add(clone);
        await _roomRepository.UpdateRoom(currentRoom);

        return $"You found a connection to {clone.Name}.";
    }
}
