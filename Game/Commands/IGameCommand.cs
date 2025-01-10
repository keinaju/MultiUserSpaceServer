using System;
using System.Text.RegularExpressions;

namespace MUS.Game.Commands;

public interface IGameCommand
{
    bool AdminOnly { get; }

    string HelpText { get; }
    
    Regex Pattern { get; }

    Task<CommandResult> Run();
}
