using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class CuriosityIsCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the curiosity of the current room.";

    public Regex Pattern => new("^curiosity is (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public CuriosityIsCommand(IInputCommand input)
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
            .CuriosityIs(RoomPoolNameInInput);
        }
    }
}
