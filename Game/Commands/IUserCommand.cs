using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;

namespace MUS.Game.Commands;

/// <summary>
/// Commands that require authenticated user to be used.
/// </summary>
public interface IUserCommand
{
    bool AdminOnly { get; }

    string HelpText { get; }
    
    Regex Pattern { get; }

    Task<CommandResult> Run(User user);
}

/// <summary>
/// Commands that do not need authenticated user to be used.
/// </summary>
public interface IUserlessCommand
{
    string HelpText { get; }
    
    Regex Pattern { get; }

    Task<CommandResult> Run();
}