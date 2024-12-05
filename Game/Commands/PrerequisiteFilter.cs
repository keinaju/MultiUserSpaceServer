using MUS.Game.Session;

namespace MUS.Game.Commands;

public enum Prerequisite
{
    UserIsLoggedIn,
    UserIsBuilder,
    UserHasSelectedBeing
}

public class PrerequisiteFilter : IPrerequisiteFilter
{
    ISessionService _session;

    public PrerequisiteFilter(
        ISessionService session
    )
    {
        _session = session;
    }

    //Returns complain string if any of prerequisites are not met
    //Returns null if all prerequisites are met
    public string? Complain(Prerequisite[] prerequisites)
    {
        if (prerequisites.Contains(Prerequisite.UserIsLoggedIn))
        {
            var user = _session.AuthenticatedUser;
            if (user is null)
            {
                return "This command has a prerequisite for a user session. "
                + " Use 'login' command to start a session.";
            }
        }

        if (prerequisites.Contains(Prerequisite.UserIsBuilder))
        {
            var user = _session.AuthenticatedUser;
            if(!user.IsBuilder)
            {
                return "This command has a prerequisite for a builder role.";
            }
        }

        if (prerequisites.Contains(Prerequisite.UserHasSelectedBeing))
        {
            var user = _session.AuthenticatedUser;
            if (user.SelectedBeing is null)
            {
                return "This command has a prerequisite for a selected being. "
                + "Use 'select' command to select a being.";
            }
        }

        return null;
    }
}
