using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewItemHatcherCommand : IGameCommand
{
    public string HelpText => "Creates a new item hatcher.";

    public Regex Regex => new("^new hatcher (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public NewItemHatcherCommand(
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
            await NewItemHatcher()
        );
    }

    private async Task<CommandResult> NewItemHatcher()
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

        return await _session.AuthenticatedUser.NewItemHatcher(item);
    }
}
