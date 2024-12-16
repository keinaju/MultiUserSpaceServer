using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;

namespace MUS.Game.Commands;

public class LoginCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
    "Requests a login token from server to establish a session.";

    private string Username => GetParameter(1);
    private string Password => GetParameter(2);

    private const string UNSUCCESSFUL_MESSAGE = "Login failed.";

    private readonly IGameResponse _response;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IHeaderDictionary _responseHeaders;

    public LoginCommand(
        IGameResponse response,
        IUserRepository userRepository,
        ITokenService tokenService,
        IHttpContextAccessor contextAccessor
    )
    : base(regex: @"^login (.+) (.+)$")
    {
        _response = response;
        _responseHeaders = contextAccessor.HttpContext.Response.Headers;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public override async Task Invoke()
    {
        var user = await _userRepository.FindUser(Username);
        if (user is null)
        {
            _response.AddText(UNSUCCESSFUL_MESSAGE);
            return;
        }

        bool result = User.VerifyPassword(Password, user.HashedPassword);
        if (result)
        {
            AddTokenHeaders(user);
            _response.AddText($"You are logged in, {Username}.");
            return;
        }

        _response.AddText(UNSUCCESSFUL_MESSAGE);
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
