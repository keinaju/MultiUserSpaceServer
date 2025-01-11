using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.New;

public class NewRoomCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Creates a new room and connects it to the current room.";

    public Regex Pattern => new("^new room (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public NewRoomCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.NewRoom(RoomNameInInput);
    }
}
