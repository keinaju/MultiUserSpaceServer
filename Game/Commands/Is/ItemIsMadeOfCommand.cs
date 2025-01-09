using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemIsMadeOfCommand : IGameCommand
{
    public string HelpText => "Sets components in a craft plan.";

    public Regex Regex => new(@"^item (.+) is made of (\d+) (.+)$");

    private string ProductNameInInput => _input.GetGroup(this.Regex, 1);

    private string QuantityInInput => _input.GetGroup(this.Regex, 2);

    private string ComponentNameInInput => _input.GetGroup(this.Regex, 3);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ItemIsMadeOfCommand(
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
            await ItemIsMadeOf()
        );
    }

    private async Task<CommandResult> ItemIsMadeOf()
    {        
        bool success = int.TryParse(QuantityInInput, out int quantity);
        if(!success || quantity < 0)
        {
            _response.AddText(
                Message.Invalid(QuantityInInput, "quantity")
            );
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.Invalid(QuantityInInput, "quantity"));
        }

        var product = await _context.FindItem(ProductNameInInput);
        if(product is null)
        {
            return ItemDoesNotExist(ProductNameInInput);
        }

        var component = await _context.FindItem(ComponentNameInInput);
        if(component is null)
        {
            return ItemDoesNotExist(ComponentNameInInput);
        }

        if(product == component)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage("Component and product can not be the same item.");
        }

        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .ItemIsMadeOf(product, component, quantity);
        }
        else
        {
            return UserIsNotSignedIn();
        }
    }
}
