using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands.Is;

public class RoomHasRequirementCommand : IUserCommand
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Sets a requirement for a feature in the current room.";

    public Regex Pattern => new("^room has requirement (.+)$");

    private string FeatureNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;
    
    public RoomHasRequirementCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        var feature = await _context.FindFeature(FeatureNameInInput);
        if(feature is null)
        {
            return CommandResult.FeatureDoesNotExist(FeatureNameInInput);
        }

        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        
        return await user.SelectedBeing.InRoom.AddFeature(feature);
    }
}
