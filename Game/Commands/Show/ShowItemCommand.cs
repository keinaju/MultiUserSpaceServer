using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;

namespace MUS.Game.Commands.Show;

public class ShowItemCommand : IGameCommand
{
    public string HelpText => "Shows details about an item.";

    public Regex Regex => new("^(show|s) item (.+)$");

    private string ItemNameInInput => _input.GetGroup(this.Regex, 2);

    private readonly GameContext _context;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public ShowItemCommand(
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
            await ShowItem()
        );
    }

    private async Task<CommandResult> ShowItem()
    {
        var item = await _context.FindItem(ItemNameInInput);
        if(item is null)
        {
            return CommandResult.ItemDoesNotExist(ItemNameInInput);
        }
        else
        {
            return item.Show();
        }
    }
}
