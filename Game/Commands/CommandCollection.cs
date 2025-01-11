using System;
using MUS.Game.Session;

namespace MUS.Game.Commands;

public class CommandCollection : ICommandCollection
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserSession _session;

    public CommandCollection(
        IServiceProvider serviceProvider,
        IUserSession session
    )
    {
        _serviceProvider = serviceProvider;
        _session = session;        
    }

    public IEnumerable<IUserCommand> GetCommands()
    {
        var commands = _serviceProvider.GetServices<IUserCommand>();

        // If the user is not admin, filter out admin commands
        if (_session.User is null || !_session.User.IsAdmin)
        {
            commands = commands.Where(command => !command.AdminOnly);
        }

        return commands;
    }
}
