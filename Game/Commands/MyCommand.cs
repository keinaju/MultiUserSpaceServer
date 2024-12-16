
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

    private readonly IGameResponse _response;
    private readonly IPlayerState _state;

    public MyCommand(IGameResponse response, IPlayerState state)
    : base(regex: @"^my$")
    {
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var being = await _state.GetBeing();

        var inventory = await _state.GetInventory();
        if(inventory.IsEmpty)
        {
            _response.AddText(
                MessageStandard.DoesNotContain(
                    $"{being.Name}'s inventory", "items"
                )
            );
            return;
        }

        _response.AddText($"{being.Name} has: {inventory.Contents()}.");
    }
}
