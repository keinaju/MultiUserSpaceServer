using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.New;

public class NewBeingCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Creates a new being.";

    public Regex Pattern => new("^new being (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public NewBeingCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.NewBeing(BeingNameInInput);
    }
}
