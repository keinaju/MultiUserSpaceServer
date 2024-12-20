﻿using MUS.Game.Data.Models;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowUserCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
        "Shows information for the currently logged in user.";

    private readonly IGameResponse _response;
    private readonly ISessionService _session;

    public ShowUserCommand(
        IGameResponse response,
        ISessionService session
    )
    : base(regex: @"^show user$")
    {
        _response = response;
        _session = session;
    }

    public override async Task Invoke()
    {
        var user = _session.AuthenticatedUser;
        if (user is null)
        {
            _response.AddText("You are not logged in.");
            return;
        }

        string output = $"You are logged in, {user.Username}. ";
        output += GetRoleText(user);
        output += GetBeingNames(user.CreatedBeings);

        _response.AddText(output);
    }

    private string GetRoleText(User user)
    {
        if(user.IsBuilder)
        {
            return "Your user has access to builder commands. ";
        }

        return string.Empty;
    }

    private string GetBeingNames(IEnumerable<Being> beings)
    {
        if(beings.Count() == 0)
        {
            return MessageStandard.DoesNotContain("Your user", "beings");
        }

        var names = new List<string>();
        foreach (var being in beings)
        {
            names.Add(being.Name!);
        }
        return $"Your user contains following beings: {MessageStandard.List(names)}.";
    }
}
