using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Repositories;

public class FeatureRepository : IFeatureRepository
{
    private readonly GameContext _context;

    public FeatureRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Feature> CreateFeature(Feature feature)
    {
        EntityEntry<Feature> entry = 
        await _context.Features.AddAsync(feature);

        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<bool> FeatureNameIsReserved(string featureName)
    {
        if(await FindFeature(featureName) is not null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<ICollection<Feature>> FindFeatures()
    {
        return await _context.Features.ToListAsync();
    }
    
    public async Task<Feature?> FindFeature(int primaryKey)
    {
        return await _context.Features.SingleAsync(
            feature => feature.PrimaryKey == primaryKey
        );
    }

    public async Task<Feature?> FindFeature(string featureName)
    {
        try
        {
            return await _context.Features
            .SingleAsync(feature => feature.Name == featureName);
        }
        catch(InvalidOperationException)
        {
            return null;
        }
    }

    public async Task<string> GetUniqueFeatureName(string featureName)
    {
        while(await FeatureNameIsReserved(featureName))
        {
            featureName += StringUtilities.GetRandomCharacter();
        }

        return featureName;
    }

    public async Task UpdateFeature(Feature updatedFeature)
    {
        var featureInDb =
        await FindFeature(updatedFeature.PrimaryKey);

        featureInDb = updatedFeature;

        await _context.SaveChangesAsync();
    }
}
