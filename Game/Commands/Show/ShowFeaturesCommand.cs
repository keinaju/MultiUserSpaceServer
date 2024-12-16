using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowFeaturesCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IFeatureRepository _featureRepository;
    private readonly IGameResponse _response;

    protected override string Description =>
        "Shows all features.";

    public ShowFeaturesCommand(
        IFeatureRepository featureRepository,
        IGameResponse response
    )
    : base(regex: @"^show features$")
    {
        _featureRepository = featureRepository;
        _response = response;
    }

    public override async Task Invoke()
    {
        var features = await _featureRepository.FindFeatures();
        if(features.Count == 0)
        {
            _response.AddText("There are no features.");
            return;
        }

        var featureNames = new List<string>();
        foreach(var feature in features)
        {
            featureNames.Add(feature.Name);
        }

        _response.AddText(
            $"Features are: {MessageStandard.List(featureNames)}."
        );
    }
}
