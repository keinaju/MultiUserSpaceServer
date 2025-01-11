using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class GoCommand : IUserCommand
{
    public bool AdminOnly => false;
    
    public string HelpText => "Moves a selected being to another room.";

    public Regex Pattern => new("^go (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public GoCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.Go(RoomNameInInput);
    }
}
