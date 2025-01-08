using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowInventoryCommand : IGameCommand
{
    public string HelpText =>
    "Shows the inventory of the current being.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^(show|s) (inventory|i)$");

    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ShowInventoryCommand(
        IResponsePayload response,
        ISessionService session
    )
    {
        _response = response;
        _session = session;
    }

    public Task Run()
    {
        _response.AddResult(
            ShowInventory()
        );

        return Task.CompletedTask;
    }

    private CommandResult ShowInventory()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return _session.AuthenticatedUser.ShowInventory();
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
