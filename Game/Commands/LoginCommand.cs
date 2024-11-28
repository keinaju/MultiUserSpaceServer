using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;

namespace MUS.Game.Commands;

public class LoginCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IHeaderDictionary _responseHeaders;

    private string Username => GetParameter(1);
    private string Password => GetParameter(2);

    private const string UNSUCCESSFUL_MESSAGE = "Login failed.";

    public LoginCommand(
        IUserRepository userRepository,
        ITokenService tokenService,
        IHttpContextAccessor contextAccessor
    )
    : base(regex: @"^login (.+) (.+)$")
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _responseHeaders = contextAccessor.HttpContext.Response.Headers;
    }

    public override async Task<string> Invoke()
    {
        var user = await _userRepository.FindUser(Username);
        if (user is null)
        {
            return UNSUCCESSFUL_MESSAGE;
        }

        bool result = User.VerifyPassword(Password, user.HashedPassword);
        if (result)
        {
            AddTokenHeaders(user);
            return $"You are logged in, {Username}.";
        }

        return UNSUCCESSFUL_MESSAGE;
    }

    private void AddTokenHeaders(User user)
    {
        string token = _tokenService.CreateToken(user.PrimaryKey.ToString());
        //Allow browser clients to access custom header 'Token'
        _responseHeaders.Append("Access-Control-Expose-Headers", "Token");
        //Provide token in header
        _responseHeaders.Append("Token", token);
    }
}
