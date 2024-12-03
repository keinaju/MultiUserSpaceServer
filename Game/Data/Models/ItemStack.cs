namespace MUS.Game.Data.Models;

public class ItemStack
{
    public int ItemPrimaryKey { get; set; }
    public required Item Item { get; set; }

    public int InventoryPrimaryKey { get; set; }
    public required Inventory Inventory { get; set; }

    public int Quantity { get; set; }

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
