using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class SellCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasSelectedBeing
    ];

    protected override string Description => 
        "Creates an offer to sell items for other items.";

    private readonly IGameResponse _response;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IOfferRepository _offerRepository;
    private readonly IPlayerState _state;

    private string SellQuantityInUserInput => GetParameter(1);
    private string SellItemName => GetParameter(2);
    private string BuyQuantityInUserInput => GetParameter(3);
    private string BuyItemName => GetParameter(4);


    public SellCommand(
        IGameResponse response,
        IInventoryRepository inventoryRepository,
        IItemRepository itemRepository,
        IOfferRepository offerRepository,
        IPlayerState state
    )
    : base(regex: @"^sell (\d+) (.+) for (\d+) (.+)$")
    {
        _inventoryRepository = inventoryRepository;
        _itemRepository = itemRepository;
        _offerRepository = offerRepository;
        _response = response;
        _state = state;
    }

    public override async Task Invoke()
    {
        var sellQuantity = GetParsedQuantity(SellQuantityInUserInput);
        if(sellQuantity is null)
        {
            _response.AddText(
                MessageStandard.Invalid(SellQuantityInUserInput, "quantity")
            );
            return;
        }

        var buyQuantity = GetParsedQuantity(BuyQuantityInUserInput);
        if(buyQuantity is null)
        {
            _response.AddText(
                MessageStandard.Invalid(BuyQuantityInUserInput, "quantity")
            );
            return;
        }

        var sellItem = await _itemRepository.FindItem(SellItemName);
        if(sellItem is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Item", SellItemName)
            );
            return;
        }
        
        var buyItem = await _itemRepository.FindItem(BuyItemName);
        if(buyItem is null)
        {
            _response.AddText(
                MessageStandard.DoesNotExist("Item", BuyItemName)
            );
            return;
        }

        if(sellItem.PrimaryKey == buyItem.PrimaryKey)
        {
            _response.AddText(
                "Offer can't contain same items."
            );
            return;
        }

        var invokingInventory = await _state.GetInventory();
        if(invokingInventory.Contains(sellItem, (int)sellQuantity) == false)
        {
            var being = await _state.GetBeing();
            _response.AddText(
                MessageStandard.DoesNotContain(
                    $"{being.Name}'s inventory",
                    MessageStandard.Quantity(sellItem.Name, (int)sellQuantity)
                )
            );
            return;
        }

        var newOffer = new Offer()
        {
            QuantityToSell = (int)sellQuantity,
            QuantityToBuy = (int)buyQuantity,
            ItemToSell = sellItem,
            ItemToBuy = buyItem,
            Inventory = invokingInventory
        };

        var matchingOffers = await _offerRepository
        .FindMatchingOffers(newOffer) as List<Offer>;
        if(matchingOffers.Count == 0)
        {
            _response.AddText(await CreateOffer(newOffer));
            return;
        }

        // Offer has matches

        // Sort to find the best deal
        matchingOffers.Sort(CompareByBuyQuantity);
        Offer? bestOffer = null;
        foreach(var offer in matchingOffers)
        {
            // Find first order that contains the required items
            var inventory = await _inventoryRepository.FindInventory(
                offer.Inventory.PrimaryKey
            );
            // Check that inventory contains required items
            if(inventory.Contains(buyItem, offer.QuantityToSell))
            {
                bestOffer = offer;
                // Adjust invoking offer to take advantage of cheapest price
                newOffer.QuantityToSell = bestOffer.QuantityToBuy;
                break;
            }
        }
        if(bestOffer is null)
        {
            _response.AddText(await CreateOffer(newOffer));
            return;
        }

        // Remove best matching offer from database
        await _offerRepository.DeleteOffer(bestOffer.PrimaryKey);

        // Switch items between inventories
        bestOffer.Inventory.TransferTo(
            invokingInventory, newOffer.ItemToBuy, newOffer.QuantityToBuy
        );
        invokingInventory.TransferTo(
            bestOffer.Inventory, newOffer.ItemToSell, newOffer.QuantityToSell
        );

        await _inventoryRepository.UpdateInventory(bestOffer.Inventory);
        await _inventoryRepository.UpdateInventory(invokingInventory);

        _response.AddText($"You sold {newOffer.ToString()}.");
        return;
    }

    private async Task<string> CreateOffer(
        Offer newOffer
    )
    {
        await _offerRepository.CreateOffer(newOffer);
        return MessageStandard.Created(
            $"offer {newOffer.ToString()}"
        );
    }

    private int CompareByBuyQuantity(Offer a, Offer b)
    {
        if (a.QuantityToBuy > b.QuantityToBuy) return 1;
        if (a.QuantityToBuy == b.QuantityToBuy) return 0;
        return -1;
    }

    private int? GetParsedQuantity(string input)
    {
        if (int.TryParse(input, out int quantity))
        {
            if (quantity > 0) return quantity;
        }

        return null;
    }
}
