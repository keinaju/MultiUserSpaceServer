using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.New;

public class NewFeatureCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new feature.";

    public Regex Pattern => new("^new feature (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private readonly IInputCommand _input;

    public NewFeatureCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.NewFeature(FeatureNameInInput);
    }
}
