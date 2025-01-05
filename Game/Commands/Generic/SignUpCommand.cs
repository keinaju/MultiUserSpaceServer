using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class SignUpCommand : IGameCommand
{
    public Condition[] Conditions => [];

    public string HelpText => "Creates a new user.";
    
    public Regex Regex => new("^sign up (.+) (.+)$");

    private string UsernameInInput =>
    _input.GetGroup(this.Regex, 1);

    private string PasswordInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;

    public SignUpCommand(
        GameContext context,
        IInputCommand input,
        IResponsePayload response
    )
    {
        _context = context;
        _input = input;
        _response = response;
    }

    public async Task Run()
    {
        _response.AddResult(
            await TrySignUp()
        );
    }

    private async Task<CommandResult> TrySignUp()
    {
        if(UsernameIsReserved())
        {
            return new CommandResult(StatusCode.Fail)
            .AddMessage(Message.ReservedName("username", UsernameInInput));
        }
        else
        {
            return await CreateNewUser();
        }
    }

    private bool UsernameIsReserved()
    {
        bool userExists = _context.Users.Any(
            user => user.Username == UsernameInInput
        );

        return userExists;
    }

    private async Task<CommandResult> CreateNewUser()
    {
        var newUser = new User()
        {
            IsBuilder = false,
            Username = UsernameInInput,
            HashedPassword = User.HashPassword(PasswordInInput)
        };

        await _context.Users.AddAsync(newUser);

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(Message.Created("user", newUser.Username));
    }
}
