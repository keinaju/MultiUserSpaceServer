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
    private readonly IObscurityRepository _obscurityRepository;
    private readonly IRoomRepository _roomRepository;

    public ExploreCommand(
        IPlayerState state,
        IObscurityRepository obscurityRepository,
        IRoomRepository roomRepository
    )
    : base(regex: @"^explore$")
    {
        _obscurityRepository = obscurityRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var currentRoom = await _state.CurrentRoom();

        var obscurity = currentRoom.Obscurity;
        if(obscurity is null)
        {
            return $"{currentRoom.Name} does not have an obscurity to explore.";
        }
        obscurity = await _obscurityRepository
            .FindObscurity(obscurity.PrimaryKey);

        var roomsInPool = obscurity.RoomPool.RoomsInPool;
        if(roomsInPool.Count == 0)
        {
            return $"Room pool {obscurity.RoomPool.Name} is empty.";
        }

        int random = new Random().Next(0, roomsInPool.Count);

        var randomRoom = roomsInPool.ToArray()[random].Room;
        // Populate obscurity
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
