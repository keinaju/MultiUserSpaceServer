using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IItemRepository
{
    Task<Item> CreateItem(Item item);
    Task<Item> FindItem(int primaryKey);
    Task<Item?> FindItem(string itemName);
    Task UpdateItem(Item updatedItem);
}
