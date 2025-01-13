using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class SelectBeingCommand : ICommandPattern
{
    public bool AdminOnly => false;
    
    public string HelpText => "Selects a being to use.";

    public Regex Pattern => new("^select (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public SelectBeingCommand(
        IInputCommand input
    )
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.SelectBeing(BeingNameInInput);
    }
}
