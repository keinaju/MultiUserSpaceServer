using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MUS.Game.Data.Models;

public class Deployment
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Item that is to be converted to being.
    /// </summary>
    public int ItemPrimaryKey { get; set; }
    public required Item Item
    {
        get => _lazyLoader.Load(this, ref _item);
        set => _item = value;
    }

    /// <summary>
    /// Being to use for cloning.
    /// </summary>
    public int PrototypePrimaryKey { get; set; }
    public required Being Prototype
    {
        get => _lazyLoader.Load(this, ref _prototype);
        set => _prototype = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private Being _prototype;
    private Item _item;

    public Deployment() {}

    private Deployment(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
}
