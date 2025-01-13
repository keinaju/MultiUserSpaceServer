using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.New;

public class NewFeatureCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new feature.";

    public Regex Pattern => new("^new feature (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public NewFeatureCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var validationResult = TextSanitation.ValidateName(FeatureNameInInput);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }

        var cleanName = TextSanitation.GetCleanName(FeatureNameInInput);
        if (await _context.FeatureNameIsReserved(cleanName))
        {
            return NameIsReserved("feature", cleanName);
        }

        await _context.Features.AddAsync(
            new Feature() { Name = cleanName }
        );

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(Message.Created("feature", cleanName));
    }
}
