using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class RoomConnectionLimitIsCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets the connection limit of the current room.";

    public Regex Pattern => new(@"^room connection limit is (\d+)$");

    private string LimitInInput => _input.GetGroup(this.Pattern, 1);

    private readonly IInputCommand _input;
    
    public RoomConnectionLimitIsCommand(IInputCommand input)
    {
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        bool ok = int.TryParse(LimitInInput, out int limit);
        if(!ok || limit < 0)
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.Invalid(LimitInInput, "connection limit"));
        }

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        else
        {
            return await user.SelectedBeing.InRoom.SetConnectionLimit(limit);
        }
    }
}
