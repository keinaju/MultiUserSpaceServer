
using MUS.Game.Data;

namespace MUS.Game.Commands;

public class MyCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IPlayerState _state;

    public MyCommand(IPlayerState state)
    : base(regex: @"^my$")
    {
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var being = await _state.Being();

        var inventory = await _state.Inventory();

        return $"{being.Name} has: {inventory.Contents()}";
    }
}
