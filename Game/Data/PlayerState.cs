using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;

namespace MUS.Game.Data;

public class PlayerState : IPlayerState
{
    private readonly IBeingRepository _beingRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ISessionService _session;

    public PlayerState(
        IBeingRepository beingRepository,
        IInventoryRepository inventoryRepository,
        IRoomRepository roomRepository,
        ISessionService session
    )
    {
        _beingRepository = beingRepository;
        _inventoryRepository = inventoryRepository;
        _roomRepository = roomRepository;
        _session = session;
    }
    
    public async Task<Being> Being()
    {
        var user = _session.AuthenticatedUser;
        if (user is null || user.PickedBeing is null)
        {
            throw new NullReferenceException();
        }

        var being = await _beingRepository.FindBeing(
            user.PickedBeing.PrimaryKey
        );
        return being;
    }

    public async Task<Inventory> Inventory()
    {
        var being = await Being();

        return await _inventoryRepository.FindInventory(
            being.Inventory.PrimaryKey
        );
    }

    public async Task<Room> Room()
    {
        var being = await Being();
        var room = await _roomRepository.FindRoom(being.Room.PrimaryKey);
        return room;
    }
}
