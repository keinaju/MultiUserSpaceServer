using System;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class GameCommandValidation : IGameCommandValidation
{
    private User? CurrentUser => _session.AuthenticatedUser;

    private Being? CurrentBeing => CurrentUser?.SelectedBeing;

    private Room? CurrentRoom => CurrentBeing?.InRoom;

    private readonly IDeploymentRepository _deployRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;
    private readonly ISessionService _session;

    public GameCommandValidation(
        IDeploymentRepository deployRepo,
        IFeatureRepository featureRepo,
        IResponsePayload response,
        IRoomRepository roomRepo,
        ISessionService session
    )
    {
        _deployRepo = deployRepo;
        _featureRepo = featureRepo;
        _response = response;
        _roomRepo = roomRepo;
        _session = session;
    }

    public bool CurrentBeingHasAllComponents(
        IEnumerable<CraftComponent> components
    )
    {
        bool success = true;

        foreach(var component in components)
        {
            if(!CurrentBeingHasItem(
                component.Quantity, component.Item
            ))
            {
                success = false;
            }
        }

        return success;
    }

    public bool CurrentBeingHasItem(int quantity, Item item)
    {
        if(!CurrentUserHasSelectedBeing())
        {
            return false;
        }

        if(CurrentBeing!.Inventory.Contains(item, quantity))
        {
            return true;
        }
        else
        {
            _response.AddText(
                Message.DoesNotHave(
                    CurrentBeing.Name,
                    Message.Quantity(item.Name, quantity)
                )
            );

            _response.Break();

            return false;
        }
    }

    public bool CurrentUserHasBeing(Being being)
    {
        if(being.CreatedByUser == CurrentUser)
        {
            return true;
        }
        else
        {
            _response.AddText(
                $"Being {being.Name} does not belong to user {CurrentUser?.Username}."
            );

            _response.Break();

            return false;
        }
    }

    public bool BeingIsNotNull(Being? being, string beingName)
    {
        if(being is null)
        {
            _response.AddText(
                Message.DoesNotExist("being", beingName)
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CurrentBeingHasFeeItem(RoomPool curiosity)
    {
        if(curiosity.FeeItem is null)
        {
            return true;
        }
        else
        {
            return CurrentBeingHasItem(1, curiosity.FeeItem);
        }
    }

    public bool CuriosityHasPrototypes(RoomPool curiosity)
    {
        if(curiosity.Prototypes.Count == 0)
        {
            _response.AddText(
                Message.DoesNotHave(curiosity.Name, "prototypes")
            );
            
            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CuriosityIsNotNull(
        RoomPool? curiosity, string roomName
    )
    {
        if(curiosity is null)
        {
            _response.AddText(
                Message.DoesNotHave(roomName, "a curiosity")
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CurrentRoomDoesNotHaveFeature(
        Feature feature, string featureName
    )
    {
        if(CurrentRoom!.RequiredFeatures.Contains(feature))
        {
            _response.AddText(
                $"{CurrentRoom.Name} already has the feature {feature.Name}."
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CurrentUserHasSelectedBeing()
    {
        if(CurrentUser?.SelectedBeing is not null)
        {
            return true;
        }
        else
        {
            _response.AddText(
                "You have not selected a being."
            );

            _response.Break();

            return false;
        }
    }

    public bool CurrentUserIsBuilder()
    {
        if (!UserIsSignedIn())
        {
            return false;
        }

        if(CurrentUser!.IsBuilder)
        {
            return true;
        }
        else
        {
            _response.AddText(
                "You do not have the builder role."
            );

            _response.Break();

            return false;
        }
    }

    public bool FeatureIsNotNull(Feature? feature, string featureName)
    {
        if(feature is null)
        {
            _response.AddText(
                Message.DoesNotExist("feature", featureName)
            );

            _response.Break();
            
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool ItemIsNotNull(Item? item, string itemName)
    {
        if(item is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", itemName)
            );

            _response.Break();
            
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool ItemHasDeployment(Item item)
    {
        if(item.Deployment is null)
        {
            _response.AddText(
                $"{item.Name} is not a deployable item."
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool ItemIsCraftable(Item item)
    {
        if(item.CraftPlan is null)
        {
            _response.AddText(
                $"{item.Name} is not a craftable item."
            );

            _response.Break();

            return false;
        }
        else if(item.CraftPlan.Components.Count == 0)
        {
            _response.AddText(
                Message.DoesNotHave(
                    $"{item.Name}'s craft plan", "components"
                )
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool QuantityIsGreaterThanZero(string quantityString)
    {
        var success = int.TryParse(
            quantityString, out int quantityInt
        );

        if(success && quantityInt > 0)
        {
            return true;
        }
        else
        {
            _response.AddText(
                Message.Invalid(quantityString, "quantity")
            );

            _response.Break();

            return false;
        }
    }

    public bool RoomDoesNotHaveBeings(Room room)
    {
        if(room.BeingsHere.Count > 0)
        {
            _response.AddText(
                Message.Has(room.Name, "beings")
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool RoomIsNotNull(Room? room, string roomName)
    {
        if(room is null)
        {
            _response.AddText(
                Message.DoesNotExist("room", roomName)
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool RoomPoolIsNotNull(RoomPool? roomPool, string roomPoolName)
    {
        if(roomPool is null)
        {
            _response.AddText(
                Message.DoesNotExist("room pool", roomPoolName)
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool UserIsSignedIn()
    {
        if(CurrentUser is null)
        {
            _response.AddText(
                "You have not signed in."
            );

            _response.Break();

            return false;
        }
        else
        {
            return true;
        }
    }
}
