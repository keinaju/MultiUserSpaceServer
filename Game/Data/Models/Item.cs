using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data.Models;

[Index(nameof(Name))]
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

    public int? DeploymentPrototypePrimaryKey { get; set; }
    public required Being? DeploymentPrototype
    {
        get => _lazyLoader.Load(this, ref _deploymentPrototype);
        set => _deploymentPrototype = value;
    }

    private readonly GameContext _context;
    private readonly ILazyLoader _lazyLoader;

    private CraftPlan? _craftPlan;
    private Being? _deploymentPrototype;

    public Item() {}

    private Item(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
    }

    public async Task<CommandResult> Deploy(Being being)
    {
        if(DeploymentPrototype is null)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage($"{Name} is not a deployable item.");
        }
        else
        {
            var clone = await being
            .CreateDeployedBeing(DeploymentPrototype);

            being.RemoveItems(1, this);

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage($"{being.Name} deployed {Name} to {clone.Name}.");
        }
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

    public bool IsDeployable()
    {
        if(DeploymentPrototype is null)
        {
            return false;
        }
        else
        {
            return true;
        }
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
        if(DeploymentPrototype is null)
        {
            return $"{Name} is not deployable.";
        }
        else
        {
            return $"{Name} can be deployed.";
        }
    }
}
