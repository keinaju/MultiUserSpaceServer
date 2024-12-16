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
        Prerequisite.UserHasSelectedBeing
    ];

    private readonly IFeatureRepository _featureRepository;
    private readonly IGameResponse _response;
    private readonly IPlayerState _state;
    private readonly IRoomRepository _roomRepository;

    private string FeatureName => GetParameter(1);

    protected override string Description =>
        "Adds a feature to the current room that a being must have to enter.";

    public AddFeatureInRoomCommand(
        IFeatureRepository featureRepository,
        IGameResponse response,
        IPlayerState state,
        IRoomRepository roomRepository
    )
    : base(regex: @"^add feature (.+) in room$")
    {
        _featureRepository = featureRepository;
        _response = response;
        _state = state;
        _roomRepository = roomRepository;
    }


    public override async Task Invoke()
    {
        var feature = await _featureRepository.FindFeature(FeatureName);
        if(feature is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Feature", FeatureName)
            );
            return;
        }

        var room = await _state.GetRoom();
        if(room.BeingMustHaveFeatures.Contains(feature))
        {
            _response.AddText(
                $"{room.Name} already has the feature {FeatureName}."
            );
            return;
        }

        room.BeingMustHaveFeatures.Add(feature);
        await _roomRepository.UpdateRoom(room);
        _response.AddText(
            $"{FeatureName} was added to {room.Name}'s must-have features."
        );
    }
}
