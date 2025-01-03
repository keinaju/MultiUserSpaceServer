using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteRoomCommand : IGameCommand
{
    public string HelpText => "Deletes a room.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^delete room (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteRoomCommand(
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
            await DeleteRoom()
        );
    }

    private async Task<CommandResult> DeleteRoom()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .DeleteRoom(RoomNameInInput);
        }
    }
}
