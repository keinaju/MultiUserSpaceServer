using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class BeingIsFeatureCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Adds a feature in the current being.";

    public Regex Pattern => new("^being feature is (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;

    public BeingIsFeatureCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await user.SelectedBeingIsFeature(FeatureNameInInput);
    }
}
