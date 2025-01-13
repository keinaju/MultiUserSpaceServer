using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.New;

public class NewItemCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new item.";

    public Regex Pattern => new("^new item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Pattern, 1);
    
    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public NewItemCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var validationResult = TextSanitation.ValidateName(ItemNameInInput);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }

        var cleanName = TextSanitation.GetCleanName(ItemNameInInput);
        if(await _context.ItemNameIsReserved(cleanName))
        {
            return NameIsReserved("item", cleanName);
        }

        await _context.Items.AddAsync(
            new Item()
            {
                CraftPlan = null,
                DeploymentPrototype = null,
                Description = null,
                Name = cleanName
            }
        );

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(Message.Created("item", cleanName));
    }
}
