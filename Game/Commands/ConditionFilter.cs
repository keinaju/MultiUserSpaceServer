using System;
using MUS.Game.Session;

namespace MUS.Game.Commands;

public class ConditionFilter : IConditionFilter
{
    private readonly IResponsePayload _response;
    private readonly ISessionService _session;

    public ConditionFilter(
        IResponsePayload response,
        ISessionService session
    )
    {
        _response = response;
        _session = session;
    }

    public bool MeetsConditions(Condition[] conditions)
    {
        if(conditions.Contains(Condition.UserIsSignedIn)
        && _session.AuthenticatedUser is null)
        {
            _response.AddText(
                "Condition: you have to sign in for this command."
            );

            return false;
        }

        if(conditions.Contains(Condition.UserIsBuilder)
        && _session.AuthenticatedUser!.IsBuilder == false)
        {
            _response.AddText(
                "Condition: you need a builder role for this command."
            );

            return false;
        }

        if(conditions.Contains(Condition.UserHasSelectedBeing)
        && _session.AuthenticatedUser!.SelectedBeing is null)
        {
            _response.AddText(
                "Condition: you have to select a being for this command."
            );

            return false;
        }

        return true;
    }
}