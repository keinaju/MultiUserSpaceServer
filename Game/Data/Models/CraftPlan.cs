using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class CraftPlan
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Item to produce if all components are present.
    /// </summary>
    public int ProductPrimaryKey { get; set; }
    public required Item Product
    {
        get => _lazyLoader.Load(this, ref _product);
        set => _product = value;
    }

    /// <summary>
    /// Collection of items and quantities needed to
    /// produce the item of the craft plan.
    /// </summary>
    public ICollection<CraftComponent> Components
    {
        get => _lazyLoader.Load(this, ref _components);
        set => _components = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private ICollection<CraftComponent> _components;
    private Item _product;

    public CraftPlan() {}

    private CraftPlan(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
    
    public void SetComponent(Item item, int quantity)
    {
        foreach(var component in Components)
        {
            if(component.Item == item)
            {
                // Craft plan already has this component
                if(quantity == 0)
                {
                    Components.Remove(component);
                }
                else
                {
                    component.Quantity = quantity;
                }
                return;
            }
        }

        if(quantity == 0)
        {
            return;
        }
        else
        {
            Components.Add(new CraftComponent() {
                Item = item,
                Quantity = quantity
            });
        }
    }

    public string? MadeOf()
    {
        if(Components.Count == 0)
        {
            return null;
        }

        var componentQuantities = new List<string>();

        foreach(var craftComponent in Components)
        {
            componentQuantities.Add(Message.Quantity(
                craftComponent.Item.Name,
                craftComponent.Quantity
            ));
        }

        return Message.List(componentQuantities);
    }
}
