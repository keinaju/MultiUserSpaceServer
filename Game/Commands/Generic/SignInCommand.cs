using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class SignInCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Creates a user session.";

    public Regex Pattern => new("^sign in (.+) (.+)$");

    private string UsernameInInput => _input.GetGroup(this.Pattern, 1);
    
    private string PasswordInInput => _input.GetGroup(this.Pattern, 2);
    
    private const string UNSUCCESSFUL_MESSAGE = "Sign in failed.";
    
    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ITokenService _tokenService;

    public SignInCommand(
        GameContext context,
        IInputCommand input,
        IResponsePayload response,
        ITokenService tokenService
    )
    {
        _context = context;
        _input = input;
        _response = response;
        _tokenService = tokenService;
    }
    
    public async Task<CommandResult> Run(User user)
    {
        return await SignIn();
    }

    public async Task<CommandResult> SignIn()
    {
        var user = await _context.FindUser(UsernameInInput);

        if(user is not null)
        {
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
