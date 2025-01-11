using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class DeploymentIsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Sets the deployment of an item to the current being.";

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    public Regex Pattern => new("^item (.+) deploy is this$");

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
        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        else
        {
            return await item.SetDeployment(user.SelectedBeing);
        }
    }
}
