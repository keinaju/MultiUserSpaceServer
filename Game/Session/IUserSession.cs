﻿using MUS.Game.Data.Models;

namespace MUS.Game.Session;

/// <summary>
/// Session service that is scoped per request.
/// </summary>
public interface IUserSession
{    
    /// <summary>
    /// Returns information about user if user is authenticated in some point of request pipeline.
    /// Returns null if user is not authenticated.
    /// </summary>
    User? User { get; }

    /// <summary>
    /// Validates token and sets user in session.
    /// </summary>
    Task AuthenticateUser(string token);
}
