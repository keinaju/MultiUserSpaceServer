using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowInventoryCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Shows the inventory of the current being.";
    
    public Regex Pattern => new("^(show|s) (inventory|i)$");

    public ShowInventoryCommand() {}

    public Task<CommandResult> Run(User user)
    {
        return Task.FromResult(user.ShowInventory());
    }
}
