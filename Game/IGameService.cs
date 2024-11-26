namespace MUS.Game;

public interface IGameService
{
    Task<string> ProcessUserInput(string userInput);
}
