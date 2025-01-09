using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Show;

public class ShowInventoryCommand : IGameCommand
{
    public string HelpText => "Shows the inventory of the current being.";
    
    public Regex Pattern => new("^(show|s) (inventory|i)$");

    private readonly ISessionService _session;

    public ShowInventoryCommand(
        ISessionService session
    )
    {
        _session = session;
    }

    public Task<CommandResult> Run()
    {
        if(_session.User is not null)
        {
            return Task.FromResult(
                _session.User.ShowInventory()
            );
        }
        else
        {
            return Task.FromResult(
                CommandResult.UserIsNotSignedIn()
            );
        }
    }
}
