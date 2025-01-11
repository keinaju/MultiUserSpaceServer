using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.New;

public class NewRoomPoolCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new room pool.";
    
    public Regex Pattern => new("^new pool (.+)$");

    private string PoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public NewRoomPoolCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.NewRoomPool(PoolNameInInput);
    }
}
