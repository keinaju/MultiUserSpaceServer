using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowFeaturesCommand : IGameCommand
{
    public string HelpText => "Shows all features.";

    public Regex Pattern => new("^(show|s) features$");

    private readonly GameContext _context;

    public ShowFeaturesCommand(
        GameContext context
    )
    {
        _context = context;
    }

    public async Task<CommandResult> Run()
    {
        var features = await _context.FindAllFeatures();
        if(features.Count == 0)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage("There are no features.");
        }
        else
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"All features are: {GetFeatureNames(features)}.");
        }
    }

    private string GetFeatureNames(IEnumerable<Feature> features)
    {
        var featureNames = new List<string>();

        foreach(var feature in features)
        {
            featureNames.Add(feature.Name);
        }

        featureNames.Sort();

        return Message.List(featureNames);
    }
}
