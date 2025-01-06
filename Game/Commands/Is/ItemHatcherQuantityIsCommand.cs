using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Is;

public class ItemHatcherQuantityIsCommand : IGameCommand
{
    public string HelpText => "Sets the quantities for an item hatcher.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new(
        @"^hatcher (.+) quantity is (\d+) to (\d+)$"
    );

    private string ItemNameInInput => _input.GetGroup(this.Regex, 1);

    private string MinimumQuantityInInput => _input.GetGroup(this.Regex, 2);

    private string MaximumQuantityInInput => _input.GetGroup(this.Regex, 3);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ItemHatcherQuantityIsCommand(
        IInputCommand input,
        IResponsePayload response,
        ISessionService session
    )
    {
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ItemHatcherQuantityIs()
        );
    }

    private async Task<CommandResult> ItemHatcherQuantityIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .ItemHatcherQuantityIs(
                ItemNameInInput,
                MinimumQuantityInInput,
                MaximumQuantityInInput
            );
        }
        else
        {
            return UserIsNotSignedIn();
        }
    }
}
