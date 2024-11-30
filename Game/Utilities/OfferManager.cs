using MUS.Game.Data.Repositories;

namespace MUS.Game.Utilities;

public class OfferManager : IOfferManager
{
    private readonly IOfferRepository _offerRepository;

    public OfferManager(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public async Task<string> FindOffers(string regex)
    {
        var offers = await _offerRepository.FindOffers(regex);
        
        if(offers.Count == 0)
        {
            return "There are no active offers.";
        }

        var offersAsStrings = new List<string>();
        foreach(var offer in offers)
        {
            offersAsStrings.Add(offer.ToString());
        }

        return 
            "Active offers are: "
            + MessageStandard.List(offersAsStrings)
            + ".";
    }
}
