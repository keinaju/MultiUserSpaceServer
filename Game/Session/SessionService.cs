using MUS.Game.Data.Models;

namespace MUS.Game.Session;

public class SessionService : ISessionService
{
    public User? AuthenticatedUser { get; set; }
}
