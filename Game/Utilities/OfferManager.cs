using System.Collections;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Utilities;

public class OfferManager : IOfferManager
{
    private readonly IOfferRepository _offerRepo;

    public OfferManager(IOfferRepository offerRepo)
    {
        _offerRepo = offerRepo;
    }

    public async Task<string> FindOffers(string regex)
    {
        var offers = await _offerRepo.FindOffers(regex);
        
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
            + Message.List(offersAsStrings)
            + ".";
    }
}
