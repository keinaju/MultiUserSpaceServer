using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewItemCommand : IGameCommand
{
    public string HelpText => "Creates a new item.";

    public Regex Regex => new("^new item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Regex, 1);
    
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public NewItemCommand(
        IInputCommand input,
        IResponsePayload response,
        ISessionService session
    )
    {
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await NewItem()
        );
    }

    private async Task<CommandResult> NewItem()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser.NewItem(ItemNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
