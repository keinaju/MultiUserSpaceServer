﻿using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IItemRepository
{
    Task<Item> CreateItem(Item item);

    Task DeleteItem(int primaryKey);

    Task<Item> FindItem(int primaryKey);

    Task<Item?> FindItem(string itemName);

    Task<ICollection<Item>> FindItems();

    Task<string> GetUniqueItemName(string itemName);

    Task<bool> ItemNameIsReserved(string itemName);

    Task UpdateItem(Item updatedItem);
}
