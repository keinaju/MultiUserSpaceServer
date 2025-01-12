using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class GameNameIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the game name.";

    public Regex Pattern => new("^game name is (.+)$");
    
    private string GameNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public GameNameIsCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var settings = await _context.GetGameSettings();
        settings.GameName = GameNameInInput;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set("game name", GameNameInInput)
        );
    }
}
