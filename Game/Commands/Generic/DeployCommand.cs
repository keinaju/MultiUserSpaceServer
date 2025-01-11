using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class DeployCommand : IUserCommand
{
    public bool AdminOnly => false;
    
    public string HelpText => "Deploys an item to a being.";

    public Regex Pattern => new("^deploy (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public DeployCommand(
        IInputCommand input
    )
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        else
        {
            return await user.SelectedBeing.DeployItem(ItemNameInInput);
        }
    }
}
