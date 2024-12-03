﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class ItemHatcherRepository : IItemHatcherRepository
{
    private readonly GameContext _context;

    public ItemHatcherRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<ItemHatcher> CreateItemHatcher(ItemHatcher itemHatcher)
    {
        EntityEntry<ItemHatcher> entry = await _context.ItemHatchers.AddAsync(itemHatcher);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<List<ItemHatcher>> FindAllItemHatchers()
    {
        return await _context.ItemHatchers
            .Include(hatcher => hatcher.Inventories)
            .Include(hatcher => hatcher.Item)
            .ToListAsync();
    }
}
