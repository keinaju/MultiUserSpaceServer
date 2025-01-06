using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemNameIsCommand : IGameCommand
{
    public string HelpText => "Renames an item.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^item (.+) name is (.+)$");

    private string OldItemNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private string NewItemNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ItemNameIsCommand(
        GameContext context,
        IInputCommand input,
        IResponsePayload response,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ItemNameIs()
        );
    }

    private async Task<CommandResult> ItemNameIs()
    {
        var item = await _context.FindItem(OldItemNameInInput);
        if(item is null)
        {
            return ItemDoesNotExist(OldItemNameInInput);
        }

        if(_session.AuthenticatedUser is null)
        {
            return UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .ItemNameIs(item, NewItemNameInInput);
        }
    }
}
