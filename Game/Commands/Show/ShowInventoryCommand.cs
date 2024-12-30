using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Show;

public class ShowInventoryCommand : IGameCommand
{
    public string HelpText =>
    "Shows the inventory of the current being.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new("^(show|s) (inventory|i)$");

    private string BeingName =>
    _player.GetSelectedBeing().Name;

    private string? InventoryContents =>
    _player.GetSelectedBeing().Inventory.Contents();

    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;

    public ShowInventoryCommand(
        IPlayerState player,
        IResponsePayload response
    )
    {
        _player = player;
        _response = response;
    }

    public Task Run()
    {
        _response.AddText(
            $"{BeingName} has {GetInventoryText()}."
        );

        return Task.CompletedTask;
    }

    private string GetInventoryText()
    {
        return InventoryContents is null ?
        "no items" : InventoryContents;
    }
}
