using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class DeploymentIsCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the deployment of an item to a being.";

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 2);

    public Regex Pattern => new("^item (.+) deploy is being (.+)$");

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public DeploymentIsCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }

        var being = await _context.FindBeing(BeingNameInInput);
        if(being is null)
        {
            return CommandResult.BeingDoesNotExist(BeingNameInInput);
        }

        return await item.SetDeployment(being);
    }
}
