using System;
using System.ComponentModel.DataAnnotations;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class CraftPlan
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Item to produce if all components are present.
    /// </summary>
    public required Item Product { get; set; }

    /// <summary>
    /// Collection of items and quantities needed to produce item of craft plan.
    /// </summary>
    public ICollection<CraftComponent> Components { get; }
        = new HashSet<CraftComponent>();
    
    public void AddComponent(Item item, int quantity)
    {
        // Strategy 1:
        // If craft plan already has this component,
        // add quantity to existing craft component quantity.
        foreach(var componentInPlan in this.Components)
        {
            if(item.PrimaryKey == componentInPlan.Item.PrimaryKey)
            {
                // The plan has already this component.
                // Add quantity to existing component:
                componentInPlan.Quantity += quantity;
                return;
            }
        }

        // Strategy 2:
        // If craft plan does not have this component,
        // create a new craft component.
        this.Components.Add(
            new CraftComponent()
            {
                Item = item,
                Quantity = quantity
            }
        );
    }

    public string IsMadeOf()
    {
        var componentQuantities = new List<string>();

        foreach(var craftComponent in Components)
        {
            componentQuantities.Add(MessageStandard.Quantity(
                craftComponent.Item.Name,
                craftComponent.Quantity
            ));
        }

        return $"{Product.Name} is made of: "
            + MessageStandard.List(componentQuantities);
    }
}
