using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

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

    private readonly GameContext _context;
    private readonly ILazyLoader _lazyLoader;
    private ICollection<Being> _associatedWithBeings;
    private ICollection<Room> _requiredInRooms;

    public Feature() {}

    private Feature(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
    }

    public async Task<CommandResult> Rename(string newName)
    {
        var validationResult = TextSanitation.ValidateName(newName);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        else
        {
            var cleanName = TextSanitation.GetCleanName(newName);

            if(await _context.FeatureNameIsReserved(cleanName))
            {
                return NameIsReserved("feature", cleanName);
            }
            else
            {
                var message = Message.Renamed(Name, cleanName);

                Name = cleanName;

                await _context.SaveChangesAsync();
                
                return new CommandResult(StatusCode.Success)
                .AddMessage(message);
            }
        }
    }
}
