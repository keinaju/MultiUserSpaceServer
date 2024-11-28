using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Rename;

public class RenameBeingCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
    ];

    private readonly IBeingRepository _beingRepository;
    private string OldBeingName => GetParameter(1);
    private string NewBeingName => GetParameter(2);

    public RenameBeingCommand(IBeingRepository beingRepository)
    : base(regex: @"^rename being (.+):(.+)$")
    {
        _beingRepository = beingRepository;
    }

    public override async Task<string> Invoke()
    {
        var being = await _beingRepository.FindBeing(OldBeingName);
        if(being is null)
        {
            return MessageStandard.DoesNotExist(OldBeingName);
        }

        being.Name = NewBeingName;
        await _beingRepository.UpdateBeing(being);

        return MessageStandard.Renamed(OldBeingName, NewBeingName);
    }
}
