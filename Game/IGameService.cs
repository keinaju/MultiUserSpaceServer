namespace MUS.Game;

public interface IGameService
{
    Task<IGameResponse> Respond(string userInput);
}
