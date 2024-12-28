using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;

namespace MUS.Game.Commands.Generic;

public class SignInCommand : IGameCommand
{
    private const string UNSUCCESSFUL_MESSAGE = "Login failed.";

    public Condition[] Conditions => [];

    public string HelpText => "Creates a user session.";

    public Regex Regex => new("^sign in (.+) (.+)$");

    private string UsernameInInput =>
    _userInput.GetGroup(this.Regex, 1);
    
    private string PasswordInInput =>
    _userInput.GetGroup(this.Regex, 2);
    
    private readonly IResponsePayload _response;
    private readonly ITokenService _tokenService;
    private readonly IUserInput _userInput;
    private readonly IUserRepository _userRepo;

    public SignInCommand(
        IResponsePayload response,
        ITokenService tokenService,
        IUserInput userInput,
        IUserRepository userRepo
    )
    {
        _response = response;
        _tokenService = tokenService;
        _userInput = userInput;
        _userRepo = userRepo;
    }
    
    public async Task Run()
    {
        var user = await _userRepo.FindUser(UsernameInInput);

        if(
            user is not null &&
            user.IsCorrectPassword(PasswordInInput)
        )
        {
            SignInSuccess(user);
        }
        else
        {
            SignInFail();
        }
    }

    private void SignInSuccess(User user)
    {
        var token = _tokenService.CreateToken(
            user.PrimaryKey.ToString()
        );
        _response.SetToken(token);

        _response.AddText(
            $"You are logged in, {user.Username}."
        );
    }

    private void SignInFail()
    {
        _response.AddText(UNSUCCESSFUL_MESSAGE);
    }
}
