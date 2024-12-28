using System;
using System.Text.RegularExpressions;

namespace MUS.Game.Commands;

public interface IGameCommand
{
    string HelpText { get; }

    Condition[] Conditions { get; }
    
    Regex Regex { get; }

    Task Run();
}
