using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands.Is;

public class RoomIsForCommand : IGameCommand
{
    public string HelpText =>
    "Sets a requirement for a feature in the current room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^room is for (.+)$");

    private Room CurrentRoom => _player.GetCurrentRoom();

    private string FeatureNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IFeatureRepository _featureRepo;
    private readonly IGameCommandValidation _validation;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly IInputCommand _input;
    
    public RoomIsForCommand(
        IFeatureRepository featureRepo,
        IGameCommandValidation validation,
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo,
        IInputCommand input
    )
    {
        _featureRepo = featureRepo;
        _validation = validation;
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
        _input = input;
    }

    public async Task Run()
    {
        var feature = await _featureRepo
        .FindFeature(FeatureNameInInput);

        if(IsValid(feature))
        {
            await AddFeatureInRoom(feature!);

            _response.AddText(
                $"{feature!.Name} was added to {CurrentRoom.Name}."
            );
        }

    }

    private bool IsValid(Feature? feature)
    {
        return _validation.UserIsSignedIn()
        && _validation.CurrentUserIsBuilder()
        && _validation.CurrentUserHasSelectedBeing()
        && _validation.FeatureIsNotNull(feature, FeatureNameInInput)
        && _validation.CurrentRoomDoesNotHaveFeature(feature!, FeatureNameInInput);
    }

    private async Task AddFeatureInRoom(Feature feature)
    {
        CurrentRoom.RequiredFeatures.Add(feature);

        await _roomRepo.UpdateRoom(CurrentRoom);
    }
}
