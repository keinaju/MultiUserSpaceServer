using System;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IFeatureRepository
{
    Task<Feature> CreateFeature(Feature feature);

    Task DeleteFeature(int primaryKey);

    Task<bool> FeatureNameIsReserved(string featureName);

    Task<Feature> FindFeature(int primaryKey);

    Task<Feature?> FindFeature(string featureName);

    Task<ICollection<Feature>> FindFeatures();

    Task<string> GetUniqueFeatureName(string featureName);    

    Task UpdateFeature(Feature updatedFeature);
}
