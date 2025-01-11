using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemIsMadeOfCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Sets components in a craft plan.";

    public Regex Pattern => new(@"^item (.+) is made of (\d+) (.+)$");

    private string ProductNameInInput => _input.GetGroup(this.Pattern, 1);

    private string QuantityInInput => _input.GetGroup(this.Pattern, 2);

    private string ComponentNameInInput => _input.GetGroup(this.Pattern, 3);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public ItemIsMadeOfCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        bool success = int.TryParse(QuantityInInput, out int quantity);
        if(!success || quantity < 0)
        {
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
        
        return await product.SetComponent(component, quantity);
    }
}
