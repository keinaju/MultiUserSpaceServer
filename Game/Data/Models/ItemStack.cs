using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MUS.Game.Data.Models;

public class ItemStack
{
    public int ItemPrimaryKey { get; set; }
    public required Item Item
    {
        get => _lazyLoader.Load(this, ref _item);
        set => _item = value;
    }

    public int InventoryPrimaryKey { get; set; }
    public required Inventory Inventory
    {
        get => _lazyLoader.Load(this, ref _inventory);
        set => _inventory = value;
    }

    public int Quantity { get; set; }

    private readonly ILazyLoader _lazyLoader;
    private Inventory _inventory;
    private Item _item;

    public ItemStack() {}

    private ItemStack(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    public ItemStack Clone()
    {
        return new ItemStack()
        {
            Item = Item,
            Inventory = Inventory,
            Quantity = Quantity
        };
    }
}
