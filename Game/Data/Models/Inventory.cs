using System.ComponentModel.DataAnnotations;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class Inventory
{
    [Key]
    public int PrimaryKey { get; set; }

    public ICollection<ItemStack> ItemStacks { get; } = new HashSet<ItemStack>();

    public ICollection<ItemGenerator> ItemGenerators { get; } = new HashSet<ItemGenerator>();

    public bool IsEmpty => ItemStacks.Count == 0;

    public void AddItems(Item item, int quantity)
    {
        ItemStack? stack = null;
        try
        {
            stack = this.ItemStacks.Single(
                stack => stack.Item.PrimaryKey == item.PrimaryKey
            );
        }
        catch(InvalidOperationException)
        {
            // Stack does not exist, so create a new one
            stack = new ItemStack()
            {
                Item = item,
                Quantity = 0,
                Inventory = this
            };

            this.ItemStacks.Add(stack);
        }

        stack.Quantity += quantity;
    }

    public Inventory Clone()
    {
        var newInventory = new Inventory() {};

        foreach(var oldStack in this.ItemStacks)
        {
            // Clone items to new inventory
            var newStack = oldStack.Clone();
            newStack.Inventory = newInventory;
            newInventory.ItemStacks.Add(newStack);
        }
        
        foreach(var generator in this.ItemGenerators)
        {
            // Refer to same item generator when room inventory is cloned
            newInventory.ItemGenerators.Add(generator);
        }

        return newInventory;
    }

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

    public void RemoveItems(Item item, int quantity)
    {
        var stack = this.ItemStacks.Single(
            stack => stack.Item.PrimaryKey == item.PrimaryKey
        );

        if(stack.Quantity == quantity)
        {
            this.ItemStacks.Remove(stack);
            return;
        }

        stack.Quantity -= quantity;
    }

    public void TransferTo(Inventory inventory, Item item, int quantity)
    {
        this.RemoveItems(item, quantity);
        inventory.AddItems(item, quantity);
    }
}
