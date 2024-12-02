using System;

namespace MUS.Game.Commands;

public class CraftCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private string ItemName => GetParameter(1);

    public CraftCommand()
    :  base(regex: @"^craft (.+)$")
    {
        
    }

    public override async Task<string> Invoke()
    {
        return "Not implemented.";
    }
}
