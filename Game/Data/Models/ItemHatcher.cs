using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class ItemHatcher
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Item to produce.
    /// </summary>
    public int ItemPrimaryKey { get; set; }
    public required Item Item
    {
        get => _lazyLoader.Load(this, ref _item);
        set => _item = value;
    }

    /// <summary>
    /// Maximum number of items to produce.
    /// </summary>
    public required int MaximumQuantity { get; set; }

    /// <summary>
    /// Minimum number of items to produce.
    /// </summary>
    public required int MinimumQuantity { get; set; }    

    /// <summary>
    /// Tick interval when items are produced.
    /// </summary>
    public required int IntervalInTicks { get; set; }

    /// <summary>
    /// A collection of inventories that have subscribed
    /// to this hatcher. Items are produced in one random
    /// inventory when tick interval expires.
    /// </summary>
    public ICollection<Inventory> Inventories
    {
        get => _lazyLoader.Load(this, ref _inventories);
        set => _inventories = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private ICollection<Inventory> _inventories;
    private Item _item;

    public ItemHatcher() {}

    private ItemHatcher(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    public string Show() =>
    $"[{MinimumQuantity} to {MaximumQuantity}] {Item.Name} "
    + $"every {Message.Quantity("ticks", IntervalInTicks)} " +
    $"in {Message.Quantity("inventories", Inventories.Count)}";
}
