using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class ExploreCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IPlayerState _state;
    private readonly IRoomPoolRepository _roomPoolRepository;
    private readonly IRoomRepository _roomRepository;

    public ExploreCommand(
        IPlayerState state,
        IRoomPoolRepository roomPoolRepository,
        IRoomRepository roomRepository
    )
    : base(regex: @"^explore$")
    {
        _roomPoolRepository = roomPoolRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var currentRoom = await _state.Room();

        var curiosity = currentRoom.Curiosity;
        if(curiosity is null)
        {
            return MessageStandard.DoesNotContain(currentRoom.Name, "a curiosity to explore");
        }
        // Populate
        curiosity = await _roomPoolRepository
            .FindRoomPool(curiosity.PrimaryKey);

        var roomsInPool = curiosity.RoomsInPool;
        if(roomsInPool.Count == 0)
        {
            return MessageStandard.DoesNotContain(curiosity.Name, "rooms");
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
