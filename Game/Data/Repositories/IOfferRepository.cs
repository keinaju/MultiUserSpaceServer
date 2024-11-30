using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IOfferRepository
{
    Task<Offer> CreateOffer(Offer offer);
    Task DeleteOffer(int primaryKey);
    Task<ICollection<Offer>> FindOffers(string regex);
    Task<ICollection<Offer>> FindMatchingOffers(Offer offer);
}
