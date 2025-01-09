using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class ItemDescriptionIsCommand : IGameCommand
{
    public string HelpText => "Sets the description of an item.";

    public Regex Regex => new("^item (.+) description is (.+)$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 1);
    
    private string DescriptionInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ItemDescriptionIsCommand(
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
            await ItemDescriptionIs()
        );
    }

    private async Task<CommandResult> ItemDescriptionIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .ItemDescriptionIs(ItemNameInInput, DescriptionInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
