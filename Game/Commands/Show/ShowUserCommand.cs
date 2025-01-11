using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowUserCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => 
    "Shows information about the user that is signed in.";

    public Regex Pattern => new("^(show|s) (user|u)$");

    public ShowUserCommand() {}

    public Task<CommandResult> Run(User user)
    {
        return Task.FromResult(user.ShowUser());
    }
}
