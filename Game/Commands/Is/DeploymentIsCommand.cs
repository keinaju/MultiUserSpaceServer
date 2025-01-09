using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class DeploymentIsCommand : IGameCommand
{
    public string HelpText =>
    "Sets the deployment of an item to the current being.";

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    public Regex Pattern => new("^item (.+) deploy is this$");

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeploymentIsCommand(
        GameContext context,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }
        if(_session.User is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.User.DeploymentIs(item);
        }
    }
}
