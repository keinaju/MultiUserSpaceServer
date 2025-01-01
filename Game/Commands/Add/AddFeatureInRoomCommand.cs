using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Add;

public class AddFeatureInRoomCommand : IGameCommand
{
    public string HelpText =>
    "Adds a requirement for a feature in the current room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^add room feature (.+)$");

    private Room CurrentRoom => _player.GetCurrentRoom();

    private string FeatureNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IFeatureRepository _featureRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IInputCommand _input;
    
    public AddFeatureInRoomCommand(
        IFeatureRepository featureRepo,
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IInputCommand input
    )
    {
        _featureRepo = featureRepo;
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
        _input = input;
    }

    public async Task Run()
    {
        var feature =
        await _featureRepo.FindFeature(FeatureNameInInput);

        if(feature is null)
        {
            _response.AddText(
                Message.DoesNotExist("feature", FeatureNameInInput)
            );

            return;
        }

        if(CurrentRoom.RequiredFeatures.Contains(feature))
        {
            _response.AddText(
                $"{CurrentRoom.Name} already has requirement for {feature.Name}."
            );

            return;
        }

        await AddFeatureInRoom(feature);

        _response.AddText(
            $"{feature.Name} was added to {CurrentRoom.Name}."
        );
    }

    private async Task AddFeatureInRoom(Feature feature)
    {
        CurrentRoom.RequiredFeatures.Add(feature);

        await _roomRepo.UpdateRoom(CurrentRoom);
    }
}
