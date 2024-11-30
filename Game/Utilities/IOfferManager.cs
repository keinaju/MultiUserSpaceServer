namespace MUS.Game.Utilities;

public interface IOfferManager
{
    Task<string> FindOffers(string regex);
}
