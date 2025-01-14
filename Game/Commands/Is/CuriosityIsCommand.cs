using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class CuriosityIsCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the curiosity of the current room.";

    public Regex Pattern => new("^room (.+) curiosity is (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private string RoomPoolNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public CuriosityIsCommand(GameContext context, IInputCommand input)
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
        
        var pool = await _context.FindRoomPool(RoomPoolNameInInput);
        if(pool is null)
        {
            return CommandResult.RoomPoolDoesNotExist(RoomPoolNameInInput);
        }

        return await room.SetCuriosity(pool);
    }
}
