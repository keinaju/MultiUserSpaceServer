using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IItemHatcherRepository
{
    Task<ItemHatcher> CreateItemHatcher(ItemHatcher itemHatcher);
    Task<List<ItemHatcher>> FindAllItemHatchers();
    Task<List<ItemHatcher>> FindItemHatchersByInventory(Inventory inventory);
    Task UpdateItemHatcher(ItemHatcher updatedItemHatcher);
}
