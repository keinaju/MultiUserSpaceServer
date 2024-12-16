using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
namespace MUS.Game.Commands;

public class SignupCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
    "Creates a user for the given username and password.";

    private string Username => GetParameter(1);
    private string Password => GetParameter(2);

    private readonly IGameResponse _response;
    private readonly IUserRepository _userRepository;

    public SignupCommand(
        IGameResponse response,
        IUserRepository userRepository
    )
    : base(regex: @"^signup (.+) (.+)$")
    {
        _response = response;
        _userRepository = userRepository;
    }

    public override async Task Invoke()
    {
        var newUser = new User()
        {
            IsBuilder = false,
            Username = Username,
            HashedPassword = User.HashPassword(Password),
        };
        var userInDb = await _userRepository.CreateUser(newUser);
        _response.AddText($"Signup succeeded, {userInDb.Username}.");
    }
}
