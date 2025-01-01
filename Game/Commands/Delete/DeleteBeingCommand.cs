using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Delete;

public class DeleteBeingCommand : IGameCommand
{
    public string HelpText => "Deletes a being.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn
    ];

    public Regex Regex => new("^delete being (.+)$");

    private string BeingNameInInput => _input.GetGroup(this.Regex, 1);

    private User CurrentUser => _session.AuthenticatedUser!;

    private readonly IBeingRepository _beingRepo;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;
    private readonly IInputCommand _input;

    public DeleteBeingCommand(
        IBeingRepository beingRepo,
        IResponsePayload response,
        ISessionService session,
        IInputCommand input
    )
    {
        _beingRepo = beingRepo;
        _response = response;
        _session = session;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await DeleteBeing();
            Respond();
        }
    }

    private async Task<bool> IsValid()
    {
        var being = await _beingRepo.FindBeing(BeingNameInInput);
        if(being is null)
        {
            _response.AddText(
                Message.DoesNotExist("being", BeingNameInInput)
            );
            return false;
        }

        if(being.CreatedByUser != CurrentUser)
        {
            _response.AddText(
                $"Your user can not delete being {BeingNameInInput}."
            );
            return false;
        }
        
        return true;
    }

    private async Task DeleteBeing()
    {
        var being = await _beingRepo.FindBeing(BeingNameInInput);

        await _beingRepo.DeleteBeing(being!.PrimaryKey);
    }

    private void Respond()
    {
        _response.AddText(
            Message.Deleted("being", BeingNameInInput)
        );
    }
}
