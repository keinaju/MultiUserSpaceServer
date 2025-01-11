using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Session;

namespace MUS.Game.Commands.Is;

public class RoomIsForCommand : IGameCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Sets a requirement for a feature in the current room.";

    public Regex Pattern => new("^room is for (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    private readonly ISessionService _session;
    
    public RoomIsForCommand(
        GameContext context,
        IInputCommand input,
        ISessionService session
    )
    {
        _context = context;
        _input = input;
        _session = session;
    }

    public async Task<CommandResult> Run()
    {
        var feature = await _context.FindFeature(FeatureNameInInput);
        if(feature is null)
        {
            return CommandResult.FeatureDoesNotExist(FeatureNameInInput);
        }

        if(_session.User is null)
        {
            return CommandResult.NotSignedInResult();
        }
        else
        {
            return await _session.User.RoomIsFor(feature);
        }
    }
}
