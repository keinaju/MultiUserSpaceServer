namespace MUS.Game.Commands;

public interface ICommandParser
{
    IGameCommand? Parse(string command);
}
