using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Session;

public class SessionService : ISessionService
{
    public User? AuthenticatedUser => _user;

    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepo;
    private User? _user = null;

    public SessionService(
        ITokenService tokenService,
        IUserRepository userRepo
    )
    {
        _tokenService = tokenService;
        _userRepo = userRepo;
    }

    public async Task AuthenticateUser(string token)
    {
        var userPrimaryKey = 
        await _tokenService.ValidateToken(token);

        var ok = int.TryParse(
            userPrimaryKey,
            out int parsedPrimaryKey
        );

        if(ok)
        {
            _user = await _userRepo.FindUser(parsedPrimaryKey);
        }
    }
}
