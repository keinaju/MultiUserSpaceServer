using MUS.Game.Data.Repositories;
using MUS.Game.Session;

namespace MUS.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenService _tokenService;
    
    public AuthenticationMiddleware(RequestDelegate next, ITokenService tokenService)
    {
        _next = next;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context, ISessionService session, IUserRepository userRepository)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault();

        if(token != null)
        {
            try
            {
                string? stringUserId = await _tokenService.ValidateToken(token);
                int intUserId = int.Parse(stringUserId);
                session.AuthenticatedUser = await userRepository.FindUser(intUserId);
            }
            catch (Exception ex)
            {
            }
        }

        await _next(context);
    }
}
