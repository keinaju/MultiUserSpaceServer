using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomIsInBeingCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Sets the current room as an inside room of the current being.";

    public Regex Pattern => new("^room is inside$");

    public RoomIsInBeingCommand() {}

    public async Task<CommandResult> Run(User user)
    {
        return await user.RoomIsInBeing();
    }
}
