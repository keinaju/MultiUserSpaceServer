using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomNameIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Renames the current room.";

    public Regex Pattern => new("^room name is (.+)$");

    private string NewNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public RoomNameIsCommand(IInputCommand input)
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
            return await user.SelectedBeing.InRoom.SetName(NewNameInInput);
        }
    }
}
