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
    
    public async Task<Being> GetBeing()
    {
        var user = _session.AuthenticatedUser;
        if (user is null || user.SelectedBeing is null)
        {
            throw new NullReferenceException();
        }

        var being = await _beingRepository.FindBeing(
            user.SelectedBeing.PrimaryKey
        );
        return being;
    }

    public async Task<Inventory> GetInventory()
    {
        var being = await GetBeing();

        return await _inventoryRepository.FindInventory(
            being.Inventory.PrimaryKey
        );
    }

    public async Task<Room> GetRoom()
    {
        var being = await GetBeing();
        var room = await _roomRepository.FindRoom(being.InRoom.PrimaryKey);
        return room;
    }

    public async Task Move(Room destination)
    {
        var being = await GetBeing();
        being.InRoom = destination;
        await _beingRepository.UpdateBeing(being);
    }
}
