using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
namespace MUS.Game.Commands;

public class SignupCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IUserRepository _userRepository;
    private string Username => GetParameter(1);
    private string Password => GetParameter(2);

    public SignupCommand(
        IUserRepository userRepository
    )
    : base(regex: @"^signup (.+) (.+)$")
    {
        _userRepository = userRepository;
    }

    public override async Task<string> Invoke()
    {
        User newUser = new()
        {
            Username = Username,
            HashedPassword = User.HashPassword(Password),
        };
        var user = await _userRepository.CreateUser(newUser);
        return $"Signup succeeded, {user.Username}.";
    }
}
