using System;
using System.Text.RegularExpressions;
using MUS.Game.Session;

namespace MUS.Game.Commands.Delete;

public class DeleteRoomPoolCommand : IGameCommand
{
    public string HelpText => "Deletes a room pool.";

    public Regex Regex => new("^delete pool (.+)$");

    private string RoomPoolNameInInput => _input.GetGroup(this.Regex, 1);

    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;

    public DeleteRoomPoolCommand(
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
            await DeleteRoomPool()
        );
    }

    private async Task<CommandResult> DeleteRoomPool()
    {
        if(_session.AuthenticatedUser is null)
        {
            return CommandResult.UserIsNotSignedIn();
        }
        else
        {
            return await _session.AuthenticatedUser
            .DeleteRoomPool(RoomPoolNameInInput);
        }
    }
}
