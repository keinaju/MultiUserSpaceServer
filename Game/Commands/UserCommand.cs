using MUS.Game.Data.Models;
using MUS.Game.Session;

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

        return $"You are logged in, {user.Username}. You have beings: {GetBeingNames(user.CreatedBeings)}.";
    }

    private string GetBeingNames(IEnumerable<Being> beings)
    {
        var names = new List<string>();

        foreach (var being in beings)
        {
            names.Add(being.Name!);
        }

        return string.Join(", ", names);
    }
}
