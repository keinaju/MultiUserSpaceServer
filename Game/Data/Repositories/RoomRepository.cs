using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;

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
        EntityEntry<Room> entry =
        await _context.Rooms.AddAsync(room);

        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task DeleteRoom(int primaryKey)
    {
        var roomInDb = await FindRoom(primaryKey);

        _context.Rooms.Remove(roomInDb);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteCuriosities(RoomPool roomPool)
    {
        var rooms = await _context.Rooms.Where(
            room => room.Curiosity == roomPool
        ).ToListAsync();

        foreach(var room in rooms)
        {
            room.Curiosity = null;
        }
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<ICollection<Room>> FindGlobalRooms()
    {
        return await _context.Rooms
        .Where(room => room.GlobalAccess == true)
        .ToListAsync();
    }

    public async Task<Room> FindRoom(int primaryKey)
    {
        return await _context.Rooms
        .SingleAsync(room => room.PrimaryKey == primaryKey);
    }

    public async Task<Room?> FindRoom(string roomName)
    {
        try
        {
            return await _context.Rooms.SingleAsync(
                room => room.Name == roomName
            );
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

    public async Task<string> GetUniqueRoomName(string name)
    {
        while(await RoomNameIsReserved(name))
        {
            name += StringUtilities.GetRandomCharacter();
        }

        return name;
    }

    public async Task<bool> RoomNameIsReserved(string roomName)
    {
        if(await FindRoom(roomName) is not null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task UpdateRoom(Room updatedRoom)
    {
        var roomInDb = await FindRoom(updatedRoom.PrimaryKey);

        roomInDb = updatedRoom;
        
        await _context.SaveChangesAsync();
    }
}