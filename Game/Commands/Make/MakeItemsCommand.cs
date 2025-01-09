using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Make;

public class MakeItemsCommand : IGameCommand
{
    public string HelpText =>
    "Creates a stack of items in the current being's inventory.";

    public Regex Regex => new(@"^make (\d+) (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Regex, 2);

    private string QuantityInInput => _input.GetGroup(this.Regex, 1);

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public MakeItemsCommand(
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
            await MakeItems()
        );
    }

    private async Task<CommandResult> MakeItems()
    {
        var ok = int.TryParse(QuantityInInput, out int quantity);
        if(!ok || quantity < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                Message.Invalid(QuantityInInput, "quantity")
            );
        }

        var item = await _context.FindItem(ItemNameInInput);
        if (item is null)
        {
            return ItemDoesNotExist(ItemNameInInput);
        }

        if(_session.AuthenticatedUser is null)
        {
            return UserIsNotSignedIn();
        }

        return await _session.AuthenticatedUser
        .MakeItems(item, quantity);
    }
}
