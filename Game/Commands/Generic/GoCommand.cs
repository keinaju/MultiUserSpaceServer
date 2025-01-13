using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class GoCommand : ICommandPattern
{
    public bool AdminOnly => false;
    
    public string HelpText => "Moves the selected being to another room.";

    public Regex Pattern => new("^go (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public GoCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
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
            var room = await _context.FindRoom(RoomNameInInput);
            if(room is null)
            {
                return CommandResult.RoomDoesNotExist(RoomNameInInput);
            }
            else
            {
                return await user.SelectedBeing.MoveTo(room);
            }
        }
    }
}
