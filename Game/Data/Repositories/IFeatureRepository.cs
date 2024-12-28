using System;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IFeatureRepository
{
    Task<Feature> CreateFeature(Feature feature);

    Task<Feature?> FindFeature(string featureName);

    Task<ICollection<Feature>> FindFeatures();

    Task UpdateFeature(Feature updatedFeature);
}
