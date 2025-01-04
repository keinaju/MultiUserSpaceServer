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

    public required Deployment? Deployment
    {
        get => _lazyLoader.Load(this, ref _deployment);
        set => _deployment = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private CraftPlan? _craftPlan;
    private Deployment? _deployment;

    public Item() {}

    private Item(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    public bool IsCraftable()
    {
        if(CraftPlan is null)
        {
            return false;
        }

        if(CraftPlan.Components.Count == 0)
        {
            return false;
        }

        return true;
    }

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

    public List<string> Show()
    {
        var texts = new List<string>();

        if(Description is not null)
        {
            texts.Add(Description);
        }
        texts.Add(GetCraftPlanText());
        texts.Add(GetDeploymentText());

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

    private string GetDeploymentText()
    {
        if(Deployment is null)
        {
            return $"{Name} is not deployable.";
        }
        else
        {
            return $"{Name} can be deployed.";
        }
    }
}
