using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class LookCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPlayerState _playerState;

    public LookCommand(
        IInventoryRepository inventoryRepository,
        IPlayerState playerState
    )
    : base(regex: @"^look$")
    {
        _inventoryRepository = inventoryRepository;
        _playerState = playerState;
    }

    public override async Task<string> Invoke()
    {
        var currentRoom = await _playerState.GetRoom(); 
        var pickedBeing = await _playerState.GetBeing();
        string outcome = $"{pickedBeing.Name} is in {currentRoom.Name}.";

        if (currentRoom.Description is not null)
        {
            outcome += $" {currentRoom.Description}";
        }

        outcome += await GetInventoryText(currentRoom);
        outcome += GetConnectionsText(currentRoom);
        outcome += GetCuriosityText(currentRoom);

        return outcome;
    }

    private string GetConnectionsText(Room room)
    {
        if (room.ConnectedToRooms.Count == 0)
        {
            return " This room has no connected rooms.";
        }
        
        var names = new List<string>();
        foreach (var connectedRoom in room.ConnectedToRooms)
        {
            names.Add(connectedRoom.Name);
        }

        return $" {room.Name} is connected to: {string.Join(", ", names)}.";
    }

    private string GetCuriosityText(Room room)
    {
        var curiosity = room.Curiosity;
        if(curiosity is null)
        {
            return "";
        }

        if(curiosity.Description != "")
        {
            return $" {curiosity.Description}";
        }
        return $" {room.Name} has a curiosity.";
    }

    private async Task<string> GetInventoryText(Room room)
    {
        // Populate with item stacks
        var inventory = await _inventoryRepository.FindInventory(
            room.Inventory.PrimaryKey
        );

        if(inventory.IsEmpty)
        {
            return $" {room.Name} has no items.";
        }

        var itemList = new List<string>();
        foreach(var stack in inventory.ItemStacks)
        {
            itemList.Add($"{stack.Item.Name} ({stack.Quantity})");
        }

        return $" {room.Name} has {inventory.ItemStacks.Count} stacks of items: "
            + $"{string.Join(", ", itemList)}.";
    }
}
