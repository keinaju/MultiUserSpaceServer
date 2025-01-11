using System;

namespace MUS.Game.Commands;

/// <summary>
/// A service to provide available commands depending 
/// on user's privileges.  A different set of commands
/// is provided for admin users than regular users.
/// </summary>
public interface ICommandCollection
{
    IEnumerable<IUserCommand> GetCommands();
}
