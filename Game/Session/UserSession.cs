using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Session;

public class UserSession : IUserSession
{
    /// <summary>
    /// User that has been authenticated.
    /// </summary>
    public User? User => _user;

    private readonly ITokenService _tokenService;
    private readonly GameContext _context;
    private User? _user = null;

    public UserSession(
        GameContext context,
        ITokenService tokenService
    )
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task AuthenticateUser(string token)
    {
        var userNameInToken = await _tokenService.ValidateToken(token);

        if(userNameInToken is not null)
        {
            _user = await _context.FindUser(userNameInToken);
        }
    }
}
