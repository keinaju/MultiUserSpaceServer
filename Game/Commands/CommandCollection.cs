using System;
using MUS.Game.Session;

namespace MUS.Game.Commands;

public class CommandCollection : ICommandCollection
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISessionService _session;

    public CommandCollection(
        IServiceProvider serviceProvider,
        ISessionService session
    )
    {
        _serviceProvider = serviceProvider;
        _session = session;        
    }

    public IEnumerable<IGameCommand> GetCommands()
    {
        var commands = _serviceProvider.GetServices<IGameCommand>();

        // If the user is not admin, filter out admin commands
        if (_session.User is null || !_session.User.IsBuilder)
        {
            commands = commands.Where(command => !command.AdminOnly);
        }

        return commands;
    }
}
