using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowRoomPoolCommand : IGameCommand
{
    public string HelpText => "Shows details about a room pool.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^(show|s) pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Regex, 2);

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public ShowRoomPoolCommand(
        GameContext context,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _context = context;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ShowRoomPool()
        );
    }

    private async Task<CommandResult> ShowRoomPool()
    {
        var pool = await _context.FindRoomPool(RoomPoolNameInInput);
        if(pool is null)
        {
            return RoomPoolDoesNotExist(RoomPoolNameInInput);
        }
        else
        {
            return pool.Show();
        }
    }
}
