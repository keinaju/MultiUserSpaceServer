using MUS.Game.Data.Models;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class UserCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly ISessionService _session;

    public UserCommand(
        ISessionService session
    )
    : base(regex: @"^user$")
    {
        _session = session;
    }

    public override async Task<string> Invoke()
    {
        var user = _session.AuthenticatedUser;
        if (user is null)
        {
            return "You are not logged in.";
        }

        string output = $"You are logged in, {user.Username}. ";
        output += GetBeingNames(user.CreatedBeings);
        return output;
    }

    private string GetBeingNames(IEnumerable<Being> beings)
    {
        if(beings.Count() == 0)
        {
            return MessageStandard.DoesNotContain("Your user", "beings");
        }

        var names = new List<string>();
        foreach (var being in beings)
        {
            names.Add(being.Name!);
        }
        return $"Your user contains following beings: {MessageStandard.List(names)}.";
    }
}
