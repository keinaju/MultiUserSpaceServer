using System;
using MUS.Game.Commands;

namespace MUS.Game.Commands;

public interface ICommandParser
{
    IEnumerable<IGameCommand> GetMatchingCommands();
}
