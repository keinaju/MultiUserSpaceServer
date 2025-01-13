using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class ExploreCommand : ICommandPattern
{
    public bool AdminOnly => false;
    
    public string HelpText => "Explores the curiosity of the current room.";

    public Regex Pattern => new("^explore$");

    public ExploreCommand() {}

    public async Task<CommandResult> Run(User user)
    {
        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        else
        {
            return await user.SelectedBeing.Explore();
        }
    }
}
