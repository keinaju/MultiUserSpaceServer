using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class Inventory
{
    [Key]
    public int PrimaryKey { get; set; }

    public ICollection<ItemHatcher> ItemHatchers
    {
        get => _lazyLoader.Load(this, ref _itemHatchers);
        set => _itemHatchers = value;
    }

    public ICollection<ItemStack> ItemStacks
    {
        get => _lazyLoader.Load(this, ref _itemStacks);
        set => _itemStacks = value;
    }

    public bool IsEmpty => ItemStacks.Count == 0;

    private readonly ILazyLoader _lazyLoader;
    private ICollection<ItemHatcher> _itemHatchers;
    private ICollection<ItemStack> _itemStacks;

    public Inventory()
    {
        ItemStacks = new List<ItemStack>();
        ItemHatchers = new List<ItemHatcher>();
    }

    private Inventory(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

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
        
        foreach(var hatcher in this.ItemHatchers)
        {
            // Refer to same item hatcher when room inventory is cloned
            newInventory.ItemHatchers.Add(hatcher);
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

    public string? Contents()
    {
        if(IsEmpty)
        {
            return null;
        }

        var itemQuantities = new List<string>();
        
        foreach(var stack in ItemStacks)
        {
            itemQuantities.Add(
                Message.Quantity(
                    stack.Item.Name,
                    stack.Quantity
                )
            );
        }

        return Message.List(itemQuantities);
    }

    /// <summary>
    /// Returns a matching item hatcher if inventory has
    /// subscribed to one.
    /// </summary>
    /// <param name="item">Item type to search for.</param>
    public ItemHatcher? GetItemHatcher(Item item)
    {
        foreach(var hatcher in ItemHatchers)
        {
            if(hatcher.Item == item)
            {
                return hatcher;
            }
        }

        return null;
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

    public void TransferTo(
        Inventory inventory,
        Item item,
        int quantity
    )
    {
        this.RemoveItems(item, quantity);
        inventory.AddItems(item, quantity);
    }
}
