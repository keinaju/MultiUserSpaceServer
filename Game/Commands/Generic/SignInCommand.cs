using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MUS.Game.Data;
using MUS.Game.Data.Models;
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
    
    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly ITokenService _tokenService;
    private readonly IInputCommand _input;

    public SignInCommand(
        GameContext context,
        IResponsePayload response,
        ITokenService tokenService,
        IInputCommand input
    )
    {
        _context = context;
        _response = response;
        _tokenService = tokenService;
        _input = input;
    }
    
    public async Task Run()
    {
        _response.AddResult(
            await TrySignIn()
        );
    }

    private async Task<CommandResult> TrySignIn()
    {
        var userExists = _context.Users.Any(
            user => user.Username == UsernameInInput
        );

        if(userExists)
        {
            var user = await _context.Users.SingleAsync(
                user => user.Username == UsernameInInput
            );

            if(user.IsCorrectPassword(PasswordInInput))
            {
                return SignInSuccess(user);
            }
        }

        return SignInFail();
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
