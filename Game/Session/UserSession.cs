﻿using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Session;

public class UserSession : IUserSession
{
    /// <summary>
    /// User that has been authenticated.
    /// </summary>
    public User? User => _user;

    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepo;
    private User? _user = null;

    public UserSession(
        ITokenService tokenService,
        IUserRepository userRepo
    )
    {
        _tokenService = tokenService;
        _userRepo = userRepo;
    }

    public async Task AuthenticateUser(string token)
    {
        var userPrimaryKey = await _tokenService.ValidateToken(token);

        var ok = int.TryParse(userPrimaryKey, out int parsedPrimaryKey);

        if(ok)
        {
            _user = await _userRepo.FindUser(parsedPrimaryKey);
        }
    }
}