using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class FeatureNameIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Renames a feature.";

    public Regex Pattern => new("^feature (.+) name is (.+)$");

    private string OldFeatureNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private string NewFeatureNameInInput => _input.GetGroup(this.Pattern, 2);

    private readonly IInputCommand _input;

    public FeatureNameIsCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.FeatureNameIs(
            OldFeatureNameInInput,
            NewFeatureNameInInput
        );
    }
}
