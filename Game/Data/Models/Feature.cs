using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MUS.Game.Data.Models;

[Index(nameof(Name))]
public class Feature
{
    [Key]
    public int PrimaryKey { get; set; }

    public required string Name { get; set; }

    public ICollection<Being> AssociatedWithBeings
    {
        get => _lazyLoader.Load(this, ref _associatedWithBeings);
        set => _associatedWithBeings = value;
    }

    public ICollection<Room> RequiredInRooms
    {
        get => _lazyLoader.Load(this, ref _requiredInRooms);
        set => _requiredInRooms = value;
    }

    private readonly ILazyLoader _lazyLoader;
    private ICollection<Being> _associatedWithBeings;
    private ICollection<Room> _requiredInRooms;

    public Feature() {}

    private Feature(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }
}
