using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class SignInCommand : IGameCommand
{
    private const string UNSUCCESSFUL_MESSAGE = "Sign in failed.";

    public Condition[] Conditions => [];

    public string HelpText => "Creates a user session.";

    public Regex Regex => new("^sign in (.+) (.+)$");

    private string UsernameInInput =>
    _input.GetGroup(this.Regex, 1);
    
    private string PasswordInInput =>
    _input.GetGroup(this.Regex, 2);
    
    private readonly IResponsePayload _response;
    private readonly ITokenService _tokenService;
    private readonly IInputCommand _input;
    private readonly IUserRepository _userRepo;

    public SignInCommand(
        IResponsePayload response,
        ITokenService tokenService,
        IInputCommand input,
        IUserRepository userRepo
    )
    {
        _response = response;
        _tokenService = tokenService;
        _input = input;
        _userRepo = userRepo;
    }
    
    public async Task Run()
    {
        _response.AddResult(
            await TrySignIn()
        );
    }

    private async Task<CommandResult> TrySignIn()
    {
        var user = await _userRepo.FindUser(UsernameInInput);

        if(
            user is not null &&
            user.IsCorrectPassword(PasswordInInput)
        )
        {
            return SignInSuccess(user);
        }
        else
        {
            return SignInFail();
        }
    }

    private CommandResult SignInSuccess(User user)
    {
        var token = _tokenService.CreateToken(
            user.PrimaryKey.ToString()
        );
        _response.SetToken(token);

        return new CommandResult(StatusCode.Success)
        .AddMessage($"You are signed in, {user.Username}.");
    }

    private CommandResult SignInFail()
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage(UNSUCCESSFUL_MESSAGE);
    }
}
