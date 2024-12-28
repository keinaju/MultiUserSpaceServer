using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class ExploreCommand : IGameCommand
{
    public string HelpText => "Explores the curiosity of the current room.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^explore$");

    private Room CurrentRoom => _player.GetCurrentRoom();

    private RoomPool? Curiosity => CurrentRoom.Curiosity;

    private Being SelectedBeing => _player.GetSelectedBeing();

    private Inventory CurrentInventory => SelectedBeing.Inventory;

    private const int MAXIMUM_NUMBER_OF_CONNECTIONS = 4;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IRoomRepository _roomRepo;

    public ExploreCommand(
        IInventoryRepository inventoryRepo,
        IPlayerState player,
        IResponsePayload response,
        IRoomRepository roomRepo
    )
    {
        _inventoryRepo = inventoryRepo;
        _player = player;
        _response = response;
        _roomRepo = roomRepo;
    }

    public async Task Run()
    {
        if(IsValid())
        {
            await Explore();
        }
    }

    private bool IsValid()
    {
        if(Curiosity is null)
        {
            _response.AddText(
                Message.DoesNotHave(CurrentRoom.Name, "a curiosity")
            );
            return false;
        }

        if(Curiosity.Prototypes.Count == 0)
        {
            _response.AddText(
                Message.DoesNotHave(Curiosity.Name, "prototypes")
            );
            return false;
        }

        if(Curiosity.FeeItem is not null)
        {
            if(!CurrentInventory.Contains(Curiosity.FeeItem, 1))
            {
                _response.AddText(
                    Message.DoesNotHave(
                        SelectedBeing.Name,
                        $"required item to explore ({Message.Quantity(Curiosity.FeeItem.Name, 1)})"
                    )
                );
                return false; 
            }
        }

        return true;
    }

    private async Task Explore()
    {
        var extensionRoom = await ExtendRoom();

        if(Curiosity!.FeeItem is not null)
        {
            await RemoveItemFromPlayer();
        }

        _response.AddText($"You found a lead to {extensionRoom.Name}.");
    }

    private async Task<Room> ExtendRoom()
    {
        var extensionRoom = Curiosity!.CreateExtensionRoom();
        extensionRoom = await _roomRepo.CreateRoom(extensionRoom);

        extensionRoom.Name = $"{extensionRoom.Name}{extensionRoom.PrimaryKey}";

        extensionRoom.ConnectBidirectionally(CurrentRoom);

        if (CurrentRoom.ConnectedToRooms.Count >= MAXIMUM_NUMBER_OF_CONNECTIONS)
        {
            CurrentRoom.Curiosity = null;
        }

        await _roomRepo.UpdateRoom(extensionRoom);
        await _roomRepo.UpdateRoom(CurrentRoom);

        return extensionRoom;
    }

    private async Task RemoveItemFromPlayer()
    {
        CurrentInventory.RemoveItems(
            Curiosity!.FeeItem!, 1
        );

        await _inventoryRepo.UpdateInventory(
            CurrentInventory
        );
    }
}
