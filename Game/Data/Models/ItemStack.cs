namespace MUS.Game.Data.Models;

public class ItemStack
{
    public int ItemPrimaryKey { get; set; }
    public Item Item { get; set; }

    public int InventoryPrimaryKey { get; set; }
    public Inventory Inventory { get; set; }

    public int Quantity { get; set; }
}
