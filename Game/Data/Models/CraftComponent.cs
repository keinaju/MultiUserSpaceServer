using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MUS.Game.Data.Models;

public class CraftComponent
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Quantity of items needed to complete a product of craft plan.
    /// </summary>
    public required int Quantity { get; set; }

    /// <summary>
    /// Item needed to complete a product of craft plan.
    /// </summary>
    public int ItemPrimaryKey { get; set; }
    public required Item Item
    {
        get => _lazyLoader.Load(this, ref _item);
        set => _item = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private Item _item;
    
    public CraftComponent() {}
    
    private CraftComponent(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
}
