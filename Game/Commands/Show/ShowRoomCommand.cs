using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowRoomCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows the current room.";

    public Regex Pattern => new("^(show|s) (room|r)$");

    public ShowRoomCommand() {}

    public Task<CommandResult> Run(User user)
    {
        return Task.FromResult(user.ShowRoom());
    }
}
