using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class SelectBeingCommand : IGameCommand
{
    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn
    ];

    public string HelpText => "Selects a being to use.";

    public Regex Regex => new("^select (.+)$");

    private string BeingNameInInput =>
    _input.GetGroup(this.Regex, 1);

    private User UserInSession => _session.AuthenticatedUser!;

    private readonly IBeingRepository _beingRepo;
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;
    private readonly IInputCommand _input;
    private readonly IUserRepository _userRepo;

    public SelectBeingCommand(
        IBeingRepository beingRepo,
        IResponsePayload response,
        ISessionService session,
        IInputCommand input,
        IUserRepository userRepo
    )
    {
        _beingRepo = beingRepo;
        _response = response;
        _session = session;
        _input = input;
        _userRepo = userRepo;
    }

    public async Task Run()
    {
        var being = await FindCorrectBeing();

        if(being is not null)
        {
            await SelectBeing(being);

            _response.AddText(
                $"You selected {being.Name}."
            );
        }
        else
        {
            _response.AddText(
                Message.DoesNotExist(
                    "being", BeingNameInInput
                )
            );
        }
    }

    private async Task<Being?> FindCorrectBeing()
    {
        var beings = await _beingRepo
        .FindBeingsByUser(_session.AuthenticatedUser!);

        return beings.Find(
            being => being.Name == BeingNameInInput
        );
    }

    private async Task SelectBeing(Being being)
    {
        UserInSession.SelectedBeing = being;
        
        await _userRepo.UpdateUser(UserInSession);
    }
}
