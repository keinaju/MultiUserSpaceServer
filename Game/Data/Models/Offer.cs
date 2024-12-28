using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class Offer
{
    [Key]
    public int PrimaryKey { get; set; }

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

    public int InventoryPrimaryKey { get; set; }
    public required Inventory Inventory
    {
        get => _lazyLoader.Load(this, ref _inventory);
        set => _inventory = value;
    }

    public override string ToString() =>
    $"({Message.Quantity(ItemToSell.Name, QuantityToSell)} "
    + $"for {Message.Quantity(ItemToBuy.Name, QuantityToBuy)})";

    private readonly ILazyLoader _lazyLoader;
    private Item _itemToBuy;
    private Item _itemToSell;
    private Inventory _inventory;

    public Offer() {}

    private Offer(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
}
