using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.New;

public class NewItemCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new item.";

    public Regex Pattern => new("^new item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private readonly IInputCommand _input;

    public NewItemCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.NewItem(ItemNameInInput);
    }
}
