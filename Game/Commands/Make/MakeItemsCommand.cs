using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Make;

public class MakeItemsCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Creates a stack of items in the current being's inventory.";

    public Regex Pattern => new(@"^make (\d+) (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 2);

    private string QuantityInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public MakeItemsCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
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

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }

        return await user.SelectedBeing.MakeItems(item, quantity);
    }
}
