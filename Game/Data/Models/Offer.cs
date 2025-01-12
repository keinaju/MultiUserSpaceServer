using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data.Models;

public class Offer
{
    [Key]
    public int PrimaryKey { get; set; }

    public int CreatedByBeingPrimaryKey { get; set; }
    public required Being CreatedByBeing
    {
        get => _lazyLoader.Load(this, ref _createdByBeing);
        set => _createdByBeing = value;
    }

    public int ItemToSellPrimaryKey { get; set; }
    public required Item ItemToSell
    {
        get => _lazyLoader.Load(this, ref _itemToSell);
        set => _itemToSell = value;
    }

    public required int QuantityToSell { get; set; }

    public int ItemToBuyPrimaryKey { get; set; }
    public required Item ItemToBuy
    {
        get => _lazyLoader.Load(this, ref _itemToBuy);
        set => _itemToBuy = value;
    }

    public required int QuantityToBuy { get; set; }

    private readonly GameContext _context;
    private readonly ILazyLoader _lazyLoader;
    private Item _itemToBuy;
    private Item _itemToSell;
    private Being _createdByBeing;

    public Offer() {}

    private Offer(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
    }

    public string GetDetails()
    {
        return $"{CreatedByBeing.Name} trades" +
        $" {Message.Quantity(ItemToSell.Name, QuantityToSell)}" +
        $" for {Message.Quantity(ItemToBuy.Name, QuantityToBuy)}";
    }

    /// <summary>
    /// Trade items with a matching offer. The calling offer has to be the offer
    /// made later, and the argument must be the offer made earlier. The order 
    /// matters because the later offer can take advantage of a lower price.
    /// </summary>
    /// <param name="earlierOffer">Earlier offer to trade the items with.</param>
    /// <returns>A command result with messages describing the transaction.</returns>
    public async Task<CommandResult> TradeItems(Offer earlierOffer)
    {
        var messages = new List<string>();

        // Orders can be matched even if later offer's sell quantity
        // does not match to earlier offer's buy quantity.
        // In this case, the later offer takes advantage
        // by decreasing it's own sell amount.
        int priceDifference = this.QuantityToSell - earlierOffer.QuantityToBuy;
        if(priceDifference > 0)
        {
            // The new offer can take advantage of an existing offer
            // by decreasing it's own sell amount.

            // Adjust the new offer to match the existing offer's price
            this.QuantityToSell -= priceDifference;

            // Take left over items back from trade inventory to free inventory
            await this.CreatedByBeing.TradeInventory.TransferTo(
                this.CreatedByBeing.FreeInventory,
                this.ItemToSell,
                priceDifference
            );

            string adjustment = Message.Quantity(this.ItemToSell.Name, priceDifference);
            string matchingBuy = Message.Quantity(earlierOffer.ItemToBuy.Name, earlierOffer.QuantityToBuy);

            messages.Add(
                $"Sell amount in {this.CreatedByBeing.Name}'s offer has been lowered by {adjustment} because the matching offer buys for {matchingBuy}."
            );
        }

        await earlierOffer.CreatedByBeing.TradeInventory.TransferTo(
            this.CreatedByBeing.FreeInventory,
            this.ItemToBuy,
            this.QuantityToBuy
        );

        await this.CreatedByBeing.TradeInventory.TransferTo(
            earlierOffer.CreatedByBeing.FreeInventory,
            this.ItemToSell,
            this.QuantityToSell
        );

        // Remove offers from database
        _context.Offers.Remove(earlierOffer);
        _context.Offers.Remove(this);
        await _context.SaveChangesAsync();

        messages.Add(
            $"Transaction has been resolved: {this.GetDetails()} with {earlierOffer.CreatedByBeing.Name}."
        );

        return new CommandResult(StatusCode.Success).AddMessages(messages);
    }
}
