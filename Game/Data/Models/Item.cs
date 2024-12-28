using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class Item
{
    [Key]
    public int PrimaryKey { get; set; }

    public required string Name { get; set; }

    public required string? Description { get; set; }

    /// <summary>
    /// Details about components that this item is made of.
    /// </summary>
    public required CraftPlan? CraftPlan
    {
        get => _lazyLoader.Load(this, ref _craftPlan);
        set => _craftPlan = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private CraftPlan? _craftPlan;

    public Item() {}

    public void SetComponent(Item item, int quantity)
    {
        if(CraftPlan is null)
        {
            CraftPlan = new CraftPlan()
            {
                Product = this,
                Components = new List<CraftComponent>()
            }; 
        }

        CraftPlan.SetComponent(item, quantity);
    }

    private Item(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    public List<string> Show()
    {
        var texts = new List<string>();

        if(Description is not null)
        {
            texts.Add(Description);
        }
        texts.Add(GetCraftPlanText());

        return texts;
    }

    private string GetCraftPlanText()
    {
        if(CraftPlan is not null)
        {
            var components = CraftPlan.MadeOf();

            if(components is null)
            {
                return Message.DoesNotHave(
                    $"{Name}'s craft plan", "components"
                );
            }
            else
            {
                return $"{Name} is made of {components}.";
            }
        }
        else
        {
            return $"{Name} is not an item that can be crafted.";
        }
    }
}
