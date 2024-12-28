using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class DeploymentRepository : IDeploymentRepository
{
    private readonly GameContext _context;

    public DeploymentRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Deployment> CreateDeployment(Deployment deployment)
    {
        EntityEntry<Deployment> entry =
        await _context.Deployments.AddAsync(deployment);

        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<Deployment?> FindDeploymentByItem(Item item)
    {
        try
        {
            return await _context.Deployments
            .Where(deploy => deploy.Item.PrimaryKey == item.PrimaryKey)
            .SingleAsync();
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }
}
