using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Generic;

public class RepeatOffersCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText => "Sets all offers of the current being to repeat mode.";

    public Regex Pattern => new("^repeat offers$");

    public async Task<CommandResult> Run(User user)
    {
        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        else
        {
            return await user.SelectedBeing.RepeatOffers();
        }
    }
}
