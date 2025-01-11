using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.New;

public class NewBeingCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new being.";

    public Regex Pattern => new("^new being (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public NewBeingCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var validationResult = TextSanitation.ValidateName(BeingNameInInput);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        
        var cleanName = TextSanitation.GetCleanName(BeingNameInInput);
        if(await _context.BeingNameIsReserved(cleanName))
        {
            return NameIsReserved("being", cleanName);
        }

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }

        var newBeing = new Being()
        {
            CreatedByUser = user,
            FreeInventory = new Inventory(),
            InRoom = user.SelectedBeing.InRoom,
            Name = cleanName,
            TradeInventory = new Inventory()
        };

        user.CreatedBeings.Add(newBeing);

        user.SelectedBeing = newBeing;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Created("being", cleanName)
            + $" {newBeing.Name} has been automatically selected."
        );
    }
}
