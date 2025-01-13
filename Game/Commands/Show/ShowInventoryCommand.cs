using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowInventoryCommand : ICommandPattern
{
    public bool AdminOnly => false;

    public string HelpText => "Shows the inventory of the current being.";
    
    public Regex Pattern => new("^(show|s) (inventory|i)$");

    public ShowInventoryCommand() {}

    public Task<CommandResult> Run(User user)
    {
        if(user.SelectedBeing is null)
        {
            return Task.FromResult(
                user.NoSelectedBeingResult()
            );
        }
        else
        {
            return Task.FromResult(
                user.SelectedBeing.ShowInventory()
            );
        }
    }
}
