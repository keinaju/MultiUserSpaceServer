using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class CraftPlanRepository : ICraftPlanRepository
{
    private readonly GameContext _context;

    public CraftPlanRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<CraftPlan> CreateCraftPlan(CraftPlan craftPlan)
    {
        EntityEntry<CraftPlan> entry =
            await _context.CraftPlans.AddAsync(craftPlan);

        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<CraftPlan?> FindCraftPlanByProduct(Item product)
    {
        try
        {
            return await _context.CraftPlans
                .Where(craftPlan =>
                    craftPlan.Product.PrimaryKey == product.PrimaryKey
                ).SingleAsync();
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }
}
