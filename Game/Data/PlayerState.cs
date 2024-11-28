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

    public async Task<Room> CurrentRoom()
    {
        var being = await PickedBeing();
        var room = await _roomRepository.FindRoom(being.Room.PrimaryKey);
        return room;
    }

    public async Task<Being> PickedBeing()
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
        var being = await PickedBeing();

        return await _inventoryRepository.FindInventory(
            being.Inventory.PrimaryKey
        );
    }
}
