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

    public async Task<CraftPlan> FindCraftPlan(int primaryKey)
    {
        return await _context.CraftPlans
            .Include(craftPlan => craftPlan.Components)
            .ThenInclude(craftComponent =>  craftComponent.Item)
            .SingleAsync(craftPlan => craftPlan.PrimaryKey == primaryKey);
    }

    public async Task<CraftPlan?> FindCraftPlanByProduct(Item product)
    {
        try
        {
            return await _context.CraftPlans
                .Include(craftPlan => craftPlan.Components)
                .ThenInclude(craftComponent =>  craftComponent.Item)
                .SingleAsync(craftPlan => 
                    craftPlan.Product.PrimaryKey == product.PrimaryKey
                );
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }

    public async Task UpdateCraftPlan(CraftPlan updatedCraftPlan)
    {
        var cpInDb = await FindCraftPlan(updatedCraftPlan.PrimaryKey);
        cpInDb = updatedCraftPlan;
        await _context.SaveChangesAsync();
    }
}
