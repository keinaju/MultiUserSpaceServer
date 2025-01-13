using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands;

/// <summary>
/// A pattern that input must match to run a command.
/// </summary>
public interface ICommandPattern
{
    bool AdminOnly { get; }

    string HelpText { get; }
    
    Regex Pattern { get; }

    Task<CommandResult> Run(User user);
}

/// <summary>
/// A command that does not require a user to run.
/// </summary>
public interface IUserlessCommand
{
    Task<CommandResult> Run();
}