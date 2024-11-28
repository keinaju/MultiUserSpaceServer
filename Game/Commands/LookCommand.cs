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

        string outcome = $"You are in {currentRoom.Name}.";

        if (currentRoom.Description is not null)
        {
            outcome += $" {currentRoom.Description}";
        }

        if (currentRoom.ConnectedToRooms.Count == 0)
        {
            outcome += $" This room has no connected rooms.";
        }
        else
        {
            outcome += $" This room is connected to: {GetRoomNames(currentRoom.ConnectedToRooms)}.";
        }

        if(currentRoom.Curiosity is not null)
        {
            outcome += " This room has a curiosity.";
        }

        return outcome;
    }

    private string GetRoomNames(IEnumerable<Room> rooms)
    {
        var names = new List<string>();

        foreach (var room in rooms)
        {
            names.Add(room.Name);
        }

        return string.Join(", ", names);
    }
}
