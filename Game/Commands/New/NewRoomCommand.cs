using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.New;

public class NewRoomCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
    ];

    public string HelpText =>
    "Creates a new room and connects it to the current room.";

    public Regex Regex => new("^new room (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public NewRoomCommand(
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
            await NewRoom()
        );
    }

    private async Task<CommandResult> NewRoom()
    {
        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser.NewRoom(RoomNameInInput);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
