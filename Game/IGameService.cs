using MUS.Game.Commands;

namespace MUS.Game;

public interface IGameService
{
    Task<CommandResult> ResolveCommand();
}
