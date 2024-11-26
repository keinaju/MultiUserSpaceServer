using MUS.Game.Data.Models;

namespace MUS.Game.Session;

/// <summary>
/// Session service that is scoped per request.
/// </summary>
public interface ISessionService
{    
    /// <summary>
    /// Returns information about user if user is authenticated in some point of request pipeline.
    /// Returns null if user is not authenticated.
    /// </summary>
    User? AuthenticatedUser { get; set; }
}
