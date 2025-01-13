using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.New;

public class NewRoomPoolCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Creates a new room pool.";
    
    public Regex Pattern => new("^new pool (.+)$");

    private string PoolNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public NewRoomPoolCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var validationResult = TextSanitation.ValidateName(PoolNameInInput);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }

        var cleanName = TextSanitation.GetCleanName(PoolNameInInput);
        if(await _context.RoomPoolNameIsReserved(cleanName))
        {
            return NameIsReserved("room pool", cleanName);
        }

        await _context.RoomPools.AddAsync(
            new RoomPool()
            {
                Description = null,
                FeeItem = null,
                Name = cleanName
            }
        );

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(Message.Created("room pool", cleanName));
    }
}
