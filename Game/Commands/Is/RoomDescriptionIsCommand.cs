using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomDescriptionIsCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the description of the current room.";

    public Regex Pattern => new("^room description is (.+)$");

    private string DescriptionInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public RoomDescriptionIsCommand(IInputCommand input)
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
            return await user.SelectedBeing.InRoom
            .SetDescription(DescriptionInInput);
        }
    }
}
