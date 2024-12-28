namespace MUS.Game;

public interface IGameService
{
    Task<IResponsePayload> Respond();
}
