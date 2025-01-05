using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
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
        var validationResult = NameSanitation.Validate(UsernameInInput);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        else
        {
            var cleanName = NameSanitation.Clean(UsernameInInput);

            if(await _context.UsernameIsReserved(cleanName))
            {
                return NameIsReserved("user", cleanName);
            }
            else
            {
                var user = new User()
                {
                    IsBuilder = false,
                    Username = cleanName,
                    HashedPassword = User.HashPassword(PasswordInInput)
                };

                return await _context.CreateUser(user);
            }
        }
    }
}
