using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowBeingCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows the currently selected being.";

    public Regex Pattern => new("^(show|s) being$");

    public ShowBeingCommand() {}

    public Task<CommandResult> Run(User user)
    {
        return Task.FromResult(user.ShowBeing());
    }
}
