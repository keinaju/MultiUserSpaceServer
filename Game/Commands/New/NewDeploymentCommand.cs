using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewDeploymentCommand : IGameCommand
{
    public string HelpText =>
    "Creates a new deployment from the current being to an item.";

    public Condition[] Conditions =>
    [
    ];

    private string ItemNameInInput => _input.GetGroup(this.Regex, 1);

    public Regex Regex => new("^new deploy (.+)$");

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public NewDeploymentCommand(
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
            await NewDeployment()
        );
    }

    private async Task<CommandResult> NewDeployment()
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

        return await _session.AuthenticatedUser.NewDeployment(item);
    }
}
