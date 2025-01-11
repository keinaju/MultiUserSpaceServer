using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Generic;

public class SignUpCommand : IUserCommand
{
    public bool AdminOnly => false;

    public string HelpText => "Creates a new user.";
    
    public Regex Pattern => new("^sign up (.+) (.+)$");

    private string UsernameInInput => _input.GetGroup(this.Pattern, 1);

    private string PasswordInInput => _input.GetGroup(this.Pattern, 2);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public SignUpCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        return await SignUpResult();
    }

    public async Task<CommandResult> SignUpResult()
    {
        var validationResult = TextSanitation.ValidateName(UsernameInInput);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        else
        {
            var cleanName = TextSanitation.GetCleanName(UsernameInInput);
            if(await _context.UsernameIsReserved(cleanName))
            {
                return NameIsReserved("user", cleanName);
            }
            else
            {
                var newUser = new User()
                {
                    IsAdmin = false,
                    Username = cleanName,
                    HashedPassword = User.HashPassword(PasswordInInput)
                };

                return await _context.CreateUser(newUser);
            }
        }
    }
}
