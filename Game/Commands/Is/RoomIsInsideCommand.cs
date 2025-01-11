using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomIsInsideCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the inside room of the current being.";

    public Regex Pattern => new("^room (.+) is inside this$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

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

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }

        return await user.SelectedBeing.SetInsideRoom(room);
    }
}
