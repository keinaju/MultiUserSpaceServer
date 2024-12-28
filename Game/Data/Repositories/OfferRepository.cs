using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly GameContext _context;

    public OfferRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Offer> CreateOffer(Offer offer)
    {
        EntityEntry<Offer> entry =
        await _context.Offers.AddAsync(offer);

        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task DeleteOffer(int primaryKey)
    {
        var offer = await _context.Offers.FindAsync(primaryKey);

        _context.Offers.Remove(offer);

        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<Offer>> FindOffers(
        string regex = ""
    )
    {
        return await _context.Offers
        .Where(offer => regex == "" ?
            true :
            Regex.IsMatch(offer.ItemToSell.Name, regex)
        )
        .ToListAsync();
    }

    public async Task<ICollection<Offer>> FindMatchingOffers(Offer newOffer)
    {
        var offers = await _context.Offers.Where(offerInDb =>
            (offerInDb.ItemToBuy.PrimaryKey == newOffer.ItemToSell.PrimaryKey)
            && (offerInDb.ItemToSell.PrimaryKey == newOffer.ItemToBuy.PrimaryKey)
            && (offerInDb.QuantityToBuy <= newOffer.QuantityToSell)
            && (offerInDb.QuantityToSell == newOffer.QuantityToBuy)
            && (offerInDb.Inventory.PrimaryKey != newOffer.Inventory.PrimaryKey)
        )
        .ToListAsync();

        return offers;
    }
}
