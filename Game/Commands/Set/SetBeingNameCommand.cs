using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Set;

public class SetBeingNameCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IPlayerState _state;
    private string BeingName => GetParameter(1);

    public SetBeingNameCommand(
        IBeingRepository beingRepository,
        IPlayerState state
    )
    : base(regex: @"^set being name (.+)$")
    {
        _beingRepository = beingRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var being = await _state.Being();
        
        var oldName = being.Name;

        being.Name = BeingName;
        await _beingRepository.UpdateBeing(being);

        return MessageStandard.Renamed(oldName, being.Name);
    }
}
