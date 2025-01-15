using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class DefaultBeingIsCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the default being of the game.";

    public Regex Pattern => new("^default being is (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public DefaultBeingIsCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var being = await _context.FindBeing(BeingNameInInput);
        if(being is null)
        {
            return CommandResult.BeingDoesNotExist(BeingNameInInput);
        }

        var settings = await _context.GetGameSettings();
        settings.DefaultBeing = being;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set("default being", being.Name)
        );
    }
}
