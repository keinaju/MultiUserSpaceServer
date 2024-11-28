using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands;

public class LookCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IPlayerState _playerState;

    public LookCommand(
        IPlayerState playerState
    )
    : base(regex: @"^look$")
    {
        _playerState = playerState;
    }

    public override async Task<string> Invoke()
    {
        var currentRoom = await _playerState.CurrentRoom(); 
        var pickedBeing = await _playerState.PickedBeing();
        string outcome = $"{pickedBeing.Name} is in {currentRoom.Name}.";

        if (currentRoom.Description is not null)
        {
            outcome += $" {currentRoom.Description}";
        }

        outcome += GetConnectionsText(currentRoom);
        outcome += GetCuriosityText(currentRoom);

        return outcome;
    }

    private string GetConnectionsText(Room currentRoom)
    {
        if (currentRoom.ConnectedToRooms.Count == 0)
        {
            return " This room has no connected rooms.";
        }
        
        var names = new List<string>();
        foreach (var room in currentRoom.ConnectedToRooms)
        {
            names.Add(room.Name);
        }

        return $" This room is connected to: {string.Join(", ", names)}.";
    }

    private string GetCuriosityText(Room currentRoom)
    {
        var curiosity = currentRoom.Curiosity;
        if(curiosity is null)
        {
            return "";
        }

        if(curiosity.Description is not null)
        {
            return $" {curiosity.Description}";
        }

        return " This room has a curiosity.";
    }
}
