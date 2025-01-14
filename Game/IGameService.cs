using MUS.Game.Commands;

namespace MUS.Game;

public interface IGameService
{
    Task ResolveCommands(string[] commands);
}
