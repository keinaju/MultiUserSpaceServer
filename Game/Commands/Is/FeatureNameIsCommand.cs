using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class FeatureNameIsCommand : IGameCommand
{
    public string HelpText => "Renames a feature.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new("^feature name (.+) is (.+)$");

    private string OldFeatureNameInInput =>
    _userInput.GetGroup(this.Regex, 1);
    
    private string NewFeatureNameInInput =>
    _userInput.GetGroup(this.Regex, 2);

    private readonly IFeatureRepository _featureRepo;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public FeatureNameIsCommand(
        IFeatureRepository featureRepo,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _featureRepo = featureRepo;
        _response = response;
        _userInput = userInput;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            Respond();
            await RenameFeature();
        }
    }

    private async Task<bool> IsValid()
    {
        var feature = await _featureRepo
        .FindFeature(OldFeatureNameInInput);

        if(feature is null)
        {
            _response.AddText(
                Message.DoesNotExist("feature", OldFeatureNameInInput)
            );
            return false;
        }        

        if(await _featureRepo.FeatureNameIsReserved(
            NewFeatureNameInInput
        ))
        {
            _response.AddText(
                Message.Reserved("feature", NewFeatureNameInInput)
            );
            return false;
        }
        
        return true;
    }

    private void Respond()
    {
        _response.AddText(
            Message.Renamed(
                OldFeatureNameInInput,
                NewFeatureNameInInput
            )
        );
    }

    private async Task RenameFeature()
    {
        var feature = await _featureRepo
        .FindFeature(OldFeatureNameInInput);

        feature!.Name = NewFeatureNameInInput;

        await _featureRepo.UpdateFeature(feature);
    }
}
