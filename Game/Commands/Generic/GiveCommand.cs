using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class GiveCommand : ICommandPattern
{
    public bool AdminOnly => false;

    public string HelpText => "Gives items to another being.";

    public Regex Pattern => new(@"^give (\d+) (.+) to (.+)$");

    private string QuantityInInput => _input.GetGroup(this.Pattern, 1);

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 2);

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 3);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public GiveCommand(GameContext context, IInputCommand input)
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }

        bool ok = int.TryParse(QuantityInInput, out int quantity);
        if(!ok || quantity < 1)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.Invalid(QuantityInInput, "quantity"));
        }
        
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }

        var receiver = await _context.FindBeing(BeingNameInInput);
        if(receiver is null)
        {
            return CommandResult.BeingDoesNotExist(BeingNameInInput);
        }

        return await user.SelectedBeing.GiveItems(item, quantity, receiver);
    }
}
