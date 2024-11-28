using MUS.Game.Data.Models;

namespace MUS.Game.Data;

public interface IPlayerState
{
    Task<Being> Being();
    Task<Room> Room();
    Task<Inventory> Inventory();
}
