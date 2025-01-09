using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Generic;

public class SellCommand : IGameCommand
{
    public string HelpText => "Creates an offer to trade items.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserHasSelectedBeing
    ];

    public Regex Regex => new(@"^sell (\d+) (.+) for (\d+) (.+)$");

    private string SellQuantityInInput =>
    _input.GetGroup(this.Regex, 1);

    private string SellItemNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private string BuyQuantityInInput =>
    _input.GetGroup(this.Regex, 3);

    private string BuyItemNameInInput =>
    _input.GetGroup(this.Regex, 4);

    private Inventory CurrentInventory =>
    _player.GetSelectedBeing().Inventory;

    private readonly IInventoryRepository _inventoryRepo;
    private readonly IItemRepository _itemRepo;
    private readonly IOfferRepository _offerRepo;
    private readonly IPlayerState _player;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    private int _validSellQuantity;
    private int _validBuyQuantity;
    private Item _validSellItem;
    private Item _validBuyItem;

    public SellCommand(
        IInventoryRepository inventoryRepo,
        IItemRepository itemRepo,
        IOfferRepository offerRepo,
        IPlayerState player,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _inventoryRepo = inventoryRepo;
        _itemRepo = itemRepo;
        _offerRepo = offerRepo;
        _player = player;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        if(await IsValid())
        {
            await ProcessOffer();
        }
    }

    private async Task<bool> IsValid()
    {
        var sellQuantity = GetParsedQuantity(SellQuantityInInput);
        if(sellQuantity is null)
        {
            _response.AddText(
                Message.Invalid(SellQuantityInInput, "quantity")
            );

            return false;
        }

        var buyQuantity = GetParsedQuantity(BuyQuantityInInput);
        if(buyQuantity is null)
        {
            _response.AddText(
                Message.Invalid(BuyQuantityInInput, "quantity")
            );

            return false;
        }

        var sellItem = await _itemRepo.FindItem(SellItemNameInInput);
        if(sellItem is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", SellItemNameInInput)
            );

            return false;
        }
        
        var buyItem = await _itemRepo.FindItem(BuyItemNameInInput);
        if(buyItem is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", BuyItemNameInInput)
            );

            return false;
        }

        if(sellItem == buyItem)
        {
            _response.AddText(
                $"The item to sell can not be the item to buy."
            );
            return false;
        }
        
        _validSellQuantity = (int)sellQuantity;
        _validBuyQuantity = (int)buyQuantity;
        _validSellItem = sellItem;
        _validBuyItem = buyItem;

        return true;
    }

    private int? GetParsedQuantity(string input)
    {
        bool success = int.TryParse(input, out int quantity);

        if (success && quantity > 0) 
        {
            return quantity;
        }

        return null;
    }

    private async Task ProcessOffer()
    {
        var newOffer = new Offer()
        {
            QuantityToSell = _validSellQuantity,
            QuantityToBuy = _validBuyQuantity,
            ItemToSell = _validSellItem,
            ItemToBuy = _validBuyItem,
            Inventory = CurrentInventory
        };

        var matchingOffer = await MatchOffer(newOffer);

        if(matchingOffer is null)
        {
            // New offer does not have a matching offer in database,
            // so save new offer in database
            await CreateOffer(newOffer);
        }
        else
        {
            // New offer does have a match in database

            // Adjust new offer to take advantage of the cheapest price
            newOffer.QuantityToSell = matchingOffer.QuantityToBuy;

            // Trade items between matching offers
            await TradeItems(
                offerToDelete: matchingOffer,
                newOffer: newOffer
            );
        }
    }

    private async Task<Offer?> MatchOffer(Offer newOffer)
    {
        var matchingOffers =
        await _offerRepo.FindMatchingOffers(newOffer)
        as List<Offer>;

        if(matchingOffers!.Count == 0)
        {
            return null;
        }
        
        // Sort to find the cheapest deal
        matchingOffers.Sort(CompareByBuyQuantity);

        foreach(var offerInDb in matchingOffers)
        {
            // Check that inventory contains required items
            if(offerInDb.Inventory.Contains(
                _validBuyItem, offerInDb.QuantityToSell
            ))
            {
                return offerInDb;
            }
        }

        return null;
    }

    private int CompareByBuyQuantity(Offer a, Offer b)
    {
        if (a.QuantityToBuy > b.QuantityToBuy)
        {
            return 1;
        }

        if (a.QuantityToBuy == b.QuantityToBuy)
        {
            return 0;
        }

        return -1;
    }

    private async Task CreateOffer(Offer offer)
    {
        await _offerRepo.CreateOffer(offer);

        _response.AddText(
            Message.Created($"offer {offer.GetDetails()}")
        );
    }

    private async Task TradeItems(Offer offerToDelete, Offer newOffer)
    {
        offerToDelete.Inventory.TransferTo(
            newOffer.Inventory,
            newOffer.ItemToBuy,
            newOffer.QuantityToBuy
        );

        newOffer.Inventory.TransferTo(
            offerToDelete.Inventory,
            newOffer.ItemToSell,
            newOffer.QuantityToSell
        );

        await _inventoryRepo.UpdateInventory(
            offerToDelete.Inventory
        );
        await _inventoryRepo.UpdateInventory(
            newOffer.Inventory
        );

        // Remove old offer from database
        await _offerRepo.DeleteOffer(offerToDelete.PrimaryKey);

        _response.AddText($"You traded {newOffer.GetDetails()}.");
    }
}
