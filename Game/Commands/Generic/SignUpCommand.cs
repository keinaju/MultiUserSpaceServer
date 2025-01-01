using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

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

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly IUserRepository _userRepo;

    public SignUpCommand(
        IResponsePayload response,
        IInputCommand input,
        IUserRepository userRepo
    )
    {
        _response = response;
        _input = input;
        _userRepo = userRepo;
    }

    public async Task Run()
    {
        if(await UsernameIsNotReserved())
        {
            await CreateNewUser();

            _response.AddText(
                Message.Created("user", UsernameInInput)
            );
        }
        else
        {
            _response.AddText(
                $"Username '{UsernameInInput}' is reserved."
            );
        }
    }

    private async Task<bool> UsernameIsNotReserved()
    {
        var user = await _userRepo.FindUser(UsernameInInput);

        if(user is null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task CreateNewUser()
    {
        var newUser = new User()
        {
            IsBuilder = false,
            Username = UsernameInInput,
            HashedPassword = User.HashPassword(PasswordInInput)
        };

        await _userRepo.CreateUser(newUser);
    }
}
