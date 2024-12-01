using MUS.Game.Data.Models;

namespace MUS.Game.Data;

public interface IPlayerState
{
    Task<Being> GetBeing();
    Task<Room> GetRoom();
    Task<Inventory> GetInventory();
    Task Move(Room destination);
}
