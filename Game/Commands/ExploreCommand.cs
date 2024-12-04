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

    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPlayerState _state;
    private readonly IRoomPoolRepository _roomPoolRepository;
    private readonly IRoomRepository _roomRepository;

    public ExploreCommand(
        IInventoryRepository inventoryRepository,
        IPlayerState state,
        IRoomPoolRepository roomPoolRepository,
        IRoomRepository roomRepository
    )
    : base(regex: @"^explore$")
    {
        _inventoryRepository = inventoryRepository;
        _roomPoolRepository = roomPoolRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var being = await _state.GetBeing();
        var currentRoom = await _state.GetRoom();
        var inventory = await _state.GetInventory();

        var curiosity = currentRoom.Curiosity;
        if(curiosity is null)
        {
            return MessageStandard.DoesNotContain(
                currentRoom.Name, "a curiosity to explore"
            );
        }
        
        // Populate
        curiosity = await _roomPoolRepository
            .FindRoomPool(curiosity.PrimaryKey);
        
        var roomsInPool = curiosity.RoomsInPool;
        if(roomsInPool.Count == 0)
        {
            return MessageStandard.DoesNotContain(
                curiosity.Name, "rooms"
            );
        }

        if(curiosity.ItemToExplore is not null)
        {
            // If player does not have the item defined
            // in room pool, prevent exploring
            if(!inventory.Contains(curiosity.ItemToExplore, 1))
            {
                return MessageStandard.DoesNotContain(
                    $"{being.Name}'s inventory",
                    $"required item to explore ({MessageStandard.Quantity(curiosity.ItemToExplore.Name, 1)})"
                );
            }

            // Otherwise, take item from player
            inventory.RemoveItems(curiosity.ItemToExplore, 1);
            await _inventoryRepository.UpdateInventory(inventory);
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
        // Prevent further exploring if room already has several connections
        if(currentRoom.ConnectedToRooms.Count >= 4)
        {
            currentRoom.Curiosity = null;
        }
        await _roomRepository.UpdateRoom(currentRoom);

        return $"You found a connection to {clone.Name}.";
    }
}
