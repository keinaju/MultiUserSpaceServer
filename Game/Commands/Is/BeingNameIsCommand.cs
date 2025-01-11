using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class BeingNameIsCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Renames the currently selected being.";

    public Regex Pattern => new("^being name is (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public BeingNameIsCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.SelectedBeingNameIs(BeingNameInInput);
    }
}
