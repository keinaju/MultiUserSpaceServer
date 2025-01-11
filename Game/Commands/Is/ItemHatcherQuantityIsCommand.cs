using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemHatcherQuantityIsCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the quantities for an item hatcher.";

    public Regex Pattern => new(@"^hatcher (.+) quantity is (\d+) to (\d+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);

    private string MinimumQuantityInInput => _input.GetGroup(this.Pattern, 2);

    private string MaximumQuantityInInput => _input.GetGroup(this.Pattern, 3);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public ItemHatcherQuantityIsCommand(
        GameContext context,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        bool minOk = int.TryParse(MinimumQuantityInInput, out int minQuantity);
        if(!minOk || minQuantity < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(
                Message.Invalid(MinimumQuantityInInput, "quantity")
            );
        }

        bool maxOk = int.TryParse(MaximumQuantityInInput, out int maxQuantity);
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
        if(item is null)
        {
            return ItemDoesNotExist(ItemNameInInput);
        }

        if(_session.User is null)
        {
            return NotSignedInResult();
        }
        else
        {
            return await _session.User
            .ItemHatcherQuantityIs(item, minQuantity, maxQuantity);
        }
    }
}
