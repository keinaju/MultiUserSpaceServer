using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class SelectBeingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    protected override string Description =>
        "Selects a being to control.";

    private readonly IBeingRepository _beingRepository;
    private readonly IGameResponse _response;
    private readonly IUserRepository _userRepository;
    private readonly ISessionService _session;
    private string BeingName => GetParameter(1);

    public SelectBeingCommand(
        IBeingRepository beingRepository,
        IGameResponse response,
        IUserRepository userRepository,
        ISessionService session
    )
    : base(regex: @"^select (.+)$")
    {
        _beingRepository = beingRepository;
        _response = response;
        _userRepository = userRepository;
        _session = session;
    }

    public override async Task Invoke()
    {
        var user = _session.AuthenticatedUser!;
        var beings = await _beingRepository.FindBeingsByUser(user);

        var being = beings.Find(being => being.Name == BeingName);
        if (being is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Being", BeingName)
            );
            return;
        }

        user.SelectedBeing = being;
        await _userRepository.UpdateUser(user);
        _response.AddText($"You selected {being.Name}.");
    }
}
