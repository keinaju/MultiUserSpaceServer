using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly GameContext _context;

    public RoomRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Room> CreateRoom(Room room)
    {
        EntityEntry<Room> entry = await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Room> FindRoom(int primaryKey)
    {
        return await _context.Rooms
            .Include(room => room.ConnectedToRooms)
            .Include(room => room.Inventory)
            .ThenInclude(inventory => inventory.ItemStacks)
            .ThenInclude(itemStack => itemStack.Item)
            .Include(room => room.Inventory)
            .ThenInclude(inventory => inventory.ItemGenerators)
            .ThenInclude(itemGenerator => itemGenerator.Item)
            .Include(room => room.Curiosity)
            .Include(room => room.BeingsHere)
            .SingleAsync(room => room.PrimaryKey == primaryKey);
    }

    public async Task<Room?> FindRoom(string roomName)
    {
        try
        {
            return await _context.Rooms
                .Include(room => room.ConnectedToRooms)
                .Include(room => room.Inventory)
                .Include(room => room.Curiosity)
                .Include(room => room.BeingsHere)
                .SingleAsync(room => room.Name == roomName);
        }
        catch (InvalidOperationException)
        {
            //Argument roomName did not match with any entity
            return null;
        }
    }

    public async Task<Room?> GetFirstRoom()
    {
        try
        {
            return await _context.Rooms.FirstAsync();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task DeleteRoom(int primaryKey)
    {
        var roomInDb = await FindRoom(primaryKey);
        _context.Rooms.Remove(roomInDb);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateRoom(Room updatedRoom)
    {
        var roomInDb = await FindRoom(updatedRoom.PrimaryKey);
        roomInDb = updatedRoom;
        await _context.SaveChangesAsync();
    }
}