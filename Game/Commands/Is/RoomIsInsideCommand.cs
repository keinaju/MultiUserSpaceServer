using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomIsInsideCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the inside room of a being.";

    public Regex Pattern => new("^room (.+) is in being (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public RoomIsInsideCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var room = await _context.FindRoom(RoomNameInInput);
        if(room is null)
        {
            return CommandResult.RoomDoesNotExist(RoomNameInInput);
        }

        var being = await _context.FindBeing(BeingNameInInput);
        if(being is null)
        {
            return CommandResult.BeingDoesNotExist(BeingNameInInput);
        }

        return await being.SetInsideRoom(room);
    }
}
