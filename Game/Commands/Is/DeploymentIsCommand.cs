using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class DeploymentIsCommand : IGameCommand
{
    public string HelpText =>
    "Sets the deployment of an item to the current being.";

    public Condition[] Conditions =>
    [
    ];

    private string ItemNameInInput => _input.GetGroup(this.Regex, 1);

    public Regex Regex => new("^item (.+) deploy is this$");

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeploymentIsCommand(
        GameContext context,
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await DeploymentIs()
        );
    }

    private async Task<CommandResult> DeploymentIs()
    {
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }

        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }

        return await _session.AuthenticatedUser.DeploymentIs(item);
    }
}
