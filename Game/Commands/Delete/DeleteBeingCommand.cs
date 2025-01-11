using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteBeingCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Deletes a being.";

    public Regex Pattern => new("^delete being (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public DeleteBeingCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.DeleteBeing(BeingNameInInput);
    }
}
