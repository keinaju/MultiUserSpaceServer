using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IInventoryRepository
{
    Task<Inventory> FindInventory(int primaryKey);
}
