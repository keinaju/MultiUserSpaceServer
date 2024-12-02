using System;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface ICraftPlanRepository
{
    Task<CraftPlan> CreateCraftPlan(CraftPlan craftPlan);
    Task<CraftPlan?> FindCraftPlanByProduct(Item product);
    Task UpdateCraftPlan(CraftPlan updatedCraftPlan);
}
