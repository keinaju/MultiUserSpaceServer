using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IItemStackRepository
{
    Task<ItemStack?> FindItemStack(int inventoryPrimaryKey, int itemPrimaryKey);

    Task SetItemStack(ItemStack itemStack);
}
