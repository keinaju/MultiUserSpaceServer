using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsForCommand : IGameCommand
{
    public string HelpText =>
    "Sets a requirement for a feature in the current room.";

    public Condition[] Conditions =>
    [
    ];

    public Regex Regex => new("^room is for (.+)$");

    private string FeatureNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;
    
    public RoomIsForCommand(
        GameContext context,
        IInputCommand input,
        IResponsePayload response,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _response = response;
        _session = session;
    }

    public async Task Run()
    {
        _response.AddResult(
            await RoomIsFor()
        );
    }

    private async Task<CommandResult> RoomIsFor()
    {
        var feature = await _context.FindFeature(FeatureNameInInput);
        if(feature is null)
        {
            return CommandResult.FeatureDoesNotExist(FeatureNameInInput);
        }

        if(_session.AuthenticatedUser is not null)
        {
            return await _session.AuthenticatedUser.RoomIsFor(feature);
        }
        else
        {
            return CommandResult.UserIsNotSignedIn();
        }
    }
}
