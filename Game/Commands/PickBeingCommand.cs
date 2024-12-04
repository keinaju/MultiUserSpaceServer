using MUS.Game.Data.Repositories;
using MUS.Game.Session;

namespace MUS.Game.Commands;

public class PickBeingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    protected override string Description =>
        "Selects a being to control.";

    private readonly IBeingRepository _beingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISessionService _session;
    private string BeingName => GetParameter(1);

    public PickBeingCommand(
        IBeingRepository beingRepository,
        IUserRepository userRepository,
        ISessionService session
    )
    : base(regex: @"^pick (.+)$")
    {
        _beingRepository = beingRepository;
        _userRepository = userRepository;
        _session = session;
    }

    public override async Task<string> Invoke()
    {
        var user = _session.AuthenticatedUser!;
        var beings = await _beingRepository.FindBeingsByUser(user);

        var being = beings.Find(being => being.Name == BeingName);
        if (being is null)
        {
            return $"'{BeingName}' does not exist.";
        }

        user.PickedBeing = being;
        await _userRepository.UpdateUser(user);
        return $"You picked {being.Name}.";
    }
}
