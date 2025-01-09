using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemHatcherQuantityIsCommand : IGameCommand
{
    public string HelpText => "Sets the quantities for an item hatcher.";

    public Regex Regex => new(
        @"^hatcher (.+) quantity is (\d+) to (\d+)$"
    );

    private string ItemNameInInput => _input.GetGroup(this.Regex, 1);

    private string MinimumQuantityInInput => _input.GetGroup(this.Regex, 2);

    private string MaximumQuantityInInput => _input.GetGroup(this.Regex, 3);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ItemHatcherQuantityIsCommand(
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
            await ItemHatcherQuantityIs()
        );
    }

    private async Task<CommandResult> ItemHatcherQuantityIs()
    {
        bool minOk = int.TryParse(
            MinimumQuantityInInput, out int minQuantity
        );
        if(!minOk || minQuantity < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                Message.Invalid(MinimumQuantityInInput, "quantity")
            );
        }

        bool maxOk = int.TryParse(
            MaximumQuantityInInput, out int maxQuantity
        );
        if(!maxOk || maxQuantity < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                Message.Invalid(MaximumQuantityInInput, "quantity")
            );
        }

        if(maxQuantity < minQuantity)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                $"Maximum quantity can not be less than minimum quantity."
            );
        }

        var item = await _context.FindItem(ItemNameInInput);
        if(item is not null)
        {
            if(_session.AuthenticatedUser is not null)
            {
                return await _session.AuthenticatedUser
                .ItemHatcherQuantityIs(item, minQuantity, maxQuantity);
            }
            else
            {
                return UserIsNotSignedIn();
            }
        }
        else
        {
            return ItemDoesNotExist(ItemNameInInput);
        }
    }
}
