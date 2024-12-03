using System;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IDeploymentRepository
{
    Task<Deployment> CreateDeployment(Deployment deployment);
    Task<Deployment?> FindDeploymentByItem(Item item);
}
