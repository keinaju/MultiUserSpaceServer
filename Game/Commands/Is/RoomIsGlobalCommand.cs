using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomIsGlobalCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Sets the global accessibility of the current room.";

    public Regex Pattern => new("^room (is|is not) global$");

    private string WordInInput => _input.GetGroup(this.Pattern, 1);

    private bool NewValue => WordInInput == "is" ? true : false;

    private readonly IInputCommand _input;

    public RoomIsGlobalCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.RoomIsGlobal(NewValue);
    }
}
