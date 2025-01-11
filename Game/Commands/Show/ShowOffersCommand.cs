using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows all offers in the current room.";

    public Regex Pattern => new("^(show|s) offers$");

    public ShowOffersCommand() {}

    public async Task<CommandResult> Run(User user)
    {
        return await user.ShowOffersInCurrentRoom();
    }
}
