using System;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class AddFeatureInRoomCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IFeatureRepository _featureRepository;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    private string FeatureName => GetParameter(1);

    protected override string Description =>
        "Adds a feature to the current room that a being must have to enter.";

    public AddFeatureInRoomCommand(
        IFeatureRepository featureRepository,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^add feature (.+) in room$")
    {
        _featureRepository = featureRepository;
        _state = state;
        _roomRepository = roomRepository;
    }


    public override async Task<string> Invoke()
    {
        var feature = await _featureRepository.FindFeature(FeatureName);
        if(feature is null)
        {
            return MessageStandard.DoesNotExist("Feature", FeatureName);
        }

        var room = await _state.GetRoom();
        if(room.BeingMustHaveFeatures.Contains(feature))
        {
            return $"{room.Name} already has the feature {FeatureName}.";
        }

        room.BeingMustHaveFeatures.Add(feature);
        await _roomRepository.UpdateRoom(room);
        return $"{FeatureName} was added to {room.Name}'s must-have features.";
    }
}
