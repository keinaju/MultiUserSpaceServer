using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class TakeCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Takes items from the current room's inventory.";

    public Regex Pattern => new("^take (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public TakeCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.TakeItem(ItemNameInInput);
    }
}
