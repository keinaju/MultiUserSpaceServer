
using MUS.Game.Data;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class MyCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description =>
        "Shows all items in the current being's inventory.";

    private readonly IPlayerState _state;

    public MyCommand(IPlayerState state)
    : base(regex: @"^my$")
    {
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var being = await _state.GetBeing();

        var inventory = await _state.GetInventory();
        if(inventory.IsEmpty)
        {
            return MessageStandard.DoesNotContain(
                $"{being.Name}'s inventory", "items"
            );
        }

        return $"{being.Name} has: {inventory.Contents()}.";
    }
}
