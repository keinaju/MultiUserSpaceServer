using System;
using MUS.Game.Session;

namespace MUS.Game.Commands;

public class CommandProvider : ICommandProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserSession _session;

    public CommandProvider(
        IServiceProvider serviceProvider,
        IUserSession session
    )
    {
        _serviceProvider = serviceProvider;
        _session = session;        
    }

    public IEnumerable<ICommandPattern> GetCommands()
    {
        var commands = _serviceProvider.GetServices<ICommandPattern>();

        // If the user is not signed in, provide only 
        // commands that do not require a user
        if (_session.User is null)
        {
            return commands.Where(command => command is IUserlessCommand);
        }
        // If the user is not admin, do not provide admin commands
        if(!_session.User.IsAdmin)
        {
            return commands.Where(command => !command.AdminOnly);
        }
        // If the user is admin, provide all commands
        else
        {
            return commands;
        }
    }
}
