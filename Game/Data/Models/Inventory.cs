using System.ComponentModel.DataAnnotations;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class Inventory
{
    [Key]
    public int PrimaryKey { get; set; }

    public ICollection<ItemStack> ItemStacks { get; set; } = new HashSet<ItemStack>();

    public ICollection<ItemGenerator> ItemGenerators { get; } = [];

    public bool IsEmpty => ItemStacks.Count == 0;

    public bool Contains(Item item, int quantity)
    {
        ItemStack? stack = null;
        try
        {
            stack = ItemStacks.Single(stack => stack.Item == item);
        }
        catch(InvalidOperationException)
        {
            return false;
        }

        if(stack.Quantity >= quantity)
        {
            return true;
        }
        return false;
    }

    public string Contents()
    {
        var itemQuantities = new List<string>();
        
        foreach(var stack in ItemStacks)
        {
            itemQuantities.Add(
                MessageStandard.Quantity(
                    stack.Item.Name,
                    stack.Quantity
                )
            );
        }

        return MessageStandard.List(itemQuantities);
    }

    public async Task TransferTo(
        Inventory destinationInventory,
        Item item,
        int quantity
    )
    {
        // Remove items from source stack
        var sourceStack = ItemStacks.Single(stack => stack.Item == item);
        if(sourceStack.Quantity == quantity)
        {
            ItemStacks.Remove(sourceStack);
        }
        else
        {
            sourceStack.Quantity -= quantity;
        }

        // Add items in destination stack
        // Ensure compatible stack exists in destination
        ItemStack? destinationStack = null;
        try
        {
            destinationStack = destinationInventory
                .ItemStacks.Single(stack => stack.Item == item);
        }
        catch(InvalidOperationException)
        {
            destinationStack = new ItemStack()
            {
                Item = item,
                Quantity = 0,
                Inventory = destinationInventory
            };

            destinationInventory.ItemStacks.Add(destinationStack);
        }
        destinationStack.Quantity += quantity;
    }
}
