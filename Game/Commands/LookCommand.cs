using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class LookCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description =>
        "Looks at the current room.";

    private readonly IGameResponse _response;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPlayerState _playerState;

    public LookCommand(
        IGameResponse response,
        IInventoryRepository inventoryRepository,
        IPlayerState playerState
    )
    : base(regex: @"^look$")
    {
        _inventoryRepository = inventoryRepository;
        _playerState = playerState;
        _response = response;
    }

    public override async Task Invoke()
    {
        var currentRoom = await _playerState.GetRoom(); 
        var selectedBeing = await _playerState.GetBeing();
        _response.AddText(
            $"{selectedBeing.Name} is in {currentRoom.Name}."
        );

        if (currentRoom.Description is not null)
        {
            _response.AddText(currentRoom.Description);
        }

        AddBeingNamesText(currentRoom, selectedBeing);
        await AddInventoryText(currentRoom);
        AddConnectionsText(currentRoom);
        AddCuriosityText(currentRoom);
    }

    private void AddBeingNamesText(Room room, Being selectedBeing)
    {
        var names = new List<string>();
        foreach(var beingHere in room.BeingsHere)
        {
            if (beingHere.PrimaryKey == selectedBeing.PrimaryKey)
            {
                // Do not add selected being's name in list
                continue;
            }

            names.Add(beingHere.Name!);
        }

        if (names.Count == 0)
        {
            return;
        }
        else if (names.Count == 1)
        {
            _response.AddText($"{names[0]} is here.");
            return;
        }

        _response.AddText($"{MessageStandard.List(names)} are here.");
    }

    private void AddConnectionsText(Room room)
    {
        if (room.ConnectedToRooms.Count == 0)
        {
            _response.AddText("This room has no connected rooms.");
            return;
        }
        
        var names = new List<string>();
        foreach (var connectedRoom in room.ConnectedToRooms)
        {
            names.Add(connectedRoom.Name);
        }

        _response.AddText(
            $"{room.Name} is connected to: {MessageStandard.List(names)}."
        );
        return;
    }

    private void AddCuriosityText(Room room)
    {
        var curiosity = room.Curiosity;
        if(curiosity is null)
        {
            return;
        }

        if(curiosity.Description != "")
        {
            _response.AddText(curiosity.Description);
            return;
        }

        _response.AddText($"{room.Name} has a curiosity.");
    }

    private async Task AddInventoryText(Room room)
    {
        // Populate with item stacks
        var inventory = await _inventoryRepository.FindInventory(
            room.Inventory.PrimaryKey
        );

        if(inventory.IsEmpty)
        {
            _response.AddText($"{room.Name} has no items.");
            return;
        }

        var itemList = new List<string>();
        foreach(var stack in inventory.ItemStacks)
        {
            itemList.Add(
                MessageStandard.Quantity(stack.Item.Name, stack.Quantity)
            );
        }

        _response.AddText(
            $"{room.Name} has {inventory.ItemStacks.Count} stacks of items: {MessageStandard.List(itemList)}."
        );
    }
}
