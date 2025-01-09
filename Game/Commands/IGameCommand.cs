using System;
using System.Text.RegularExpressions;

namespace MUS.Game.Commands;

public interface IGameCommand
{
    string HelpText { get; }
    
    Regex Pattern { get; }

    Task<CommandResult> Run();
}
