using System;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands;

public interface IGameCommandValidation
{
    bool BeingIsNotNull(Being? being, string beingName);

    bool CuriosityHasPrototypes(RoomPool curiosity);

    bool CuriosityIsNotNull(RoomPool? curiosity, string roomName);

    bool CurrentBeingHasFeeItem(RoomPool curiosity);
    
    bool CurrentBeingHasAllComponents(IEnumerable<CraftComponent> components);

    bool CurrentBeingHasItem(int quantity, Item item);

    bool CurrentRoomDoesNotHaveFeature(Feature feature, string featureName);

    bool CurrentUserHasBeing(Being being);

    bool CurrentUserHasSelectedBeing();

    bool CurrentUserIsBuilder();

    bool FeatureIsNotNull(Feature? feature, string featureName);

    bool ItemIsNotNull(Item? item, string itemName);

    bool ItemHasDeployment(Item item);

    bool ItemIsCraftable(Item item);

    bool RoomDoesNotHaveBeings(Room room);

    bool RoomIsNotNull(Room? room, string roomName);

    bool RoomPoolIsNotNull(RoomPool? roomPool, string roomPoolName);

    bool QuantityIsGreaterThanZero(string quantity);

    bool UserIsSignedIn();
}
