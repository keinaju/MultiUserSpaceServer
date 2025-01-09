using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomDescriptionIsCommand : IGameCommand
{
    public string HelpText =>
    "Sets the description of the current room.";

    public Regex Regex => new("^room description is (.+)$");

    private string DescriptionInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public RoomDescriptionIsCommand(
        IResponsePayload response,
        IInputCommand input,
        ISessionService session
    )
    {
        _response = response;
        _input = input;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await RoomDescriptionIs()
        );
    }

    private async Task<CommandResult> RoomDescriptionIs()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser
            .RoomDescriptionIs(DescriptionInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
