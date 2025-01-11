using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class ItemDescriptionIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the description of an item.";

    public Regex Pattern => new("^item (.+) description is (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private string DescriptionInInput => _input.GetGroup(this.Pattern, 2);

    private readonly IInputCommand _input;

    public ItemDescriptionIsCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.ItemDescriptionIs(
            ItemNameInInput,
            DescriptionInInput
        );
    }
}
