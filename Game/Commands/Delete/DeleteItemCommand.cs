using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteItemCommand : IGameCommand
{
    public string HelpText => "Deletes an item.";

    public Regex Regex => new("^delete item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteItemCommand(
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await DeleteItem()
        );
    }

    private async Task<CommandResult> DeleteItem()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .DeleteItem(ItemNameInInput);
        }
    }
}
