using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class TradeCommand : ICommandPattern
{
    public bool AdminOnly => false;

    public string HelpText => "Creates an offer to trade items.";

    public Regex Pattern => new(@"^(trade) (\d+) (.+) for (\d+) (.+)$");

    private string SellQuantityInInput => _input.GetGroup(this.Pattern, 2);

    private string SellItemNameInInput => _input.GetGroup(this.Pattern, 3);

    private string BuyQuantityInInput => _input.GetGroup(this.Pattern, 4);

    private string BuyItemNameInInput => _input.GetGroup(this.Pattern, 5);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public TradeCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        bool buyOk = int.TryParse(BuyQuantityInInput, out int buyQuantity);
        if(!buyOk || buyQuantity < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.Invalid(BuyQuantityInInput, "quantity"));
        }

        bool sellOk = int.TryParse(SellQuantityInInput, out int sellQuantity);
        if(!sellOk || sellQuantity < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.Invalid(SellQuantityInInput, "quantity"));
        }

        var sellItem = await _context.FindItem(SellItemNameInInput);
        if(sellItem is null)
        {
            return ItemDoesNotExist(SellItemNameInInput);
        }

        var buyItem = await _context.FindItem(BuyItemNameInInput);
        if(buyItem is null)
        {
            return ItemDoesNotExist(BuyItemNameInInput);
        }

        if(sellItem == buyItem)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage("The item to sell can not be the item to buy.");
        }

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }

        return await user.SelectedBeing.Offer(
            sellQuantity: sellQuantity,
            buyQuantity: buyQuantity,
            sellItem: sellItem,
            buyItem: buyItem,
            repeatMode: false
        );
    }
}
