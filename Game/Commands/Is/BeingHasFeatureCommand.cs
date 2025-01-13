using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class BeingHasFeatureCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Adds a feature in the current being.";

    public Regex Pattern => new("^being has feature (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public BeingHasFeatureCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var feature = await _context.FindFeature(FeatureNameInInput);
        if(feature is null)
        {
            return CommandResult.FeatureDoesNotExist(FeatureNameInInput);
        }

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        
        return await user.SelectedBeing.AddFeature(feature);
    }
}
