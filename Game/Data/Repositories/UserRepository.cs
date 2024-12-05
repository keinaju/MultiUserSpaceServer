using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GameContext _context;

    public UserRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUser(User user)
    {
        EntityEntry<User> entry = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<User> FindUser(int primaryKey)
    {
        return await _context.Users
            .Include(user => user.SelectedBeing)
            .Include(user => user.CreatedBeings)
            .SingleAsync(user => user.PrimaryKey == primaryKey);
    }

    public async Task<User?> FindUser(string username)
    {
        try
        {
            return await _context.Users
                .Include(user => user.SelectedBeing)
                .Include(user => user.CreatedBeings)
                .SingleAsync(user => user.Username == username);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task DeleteUser(int primaryKey)
    {
        var userInDb = await FindUser(primaryKey);
        _context.Users.Remove(userInDb);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(User updatedUser)
    {
        var userInDb = await FindUser(updatedUser.PrimaryKey);
        userInDb = updatedUser;
        await _context.SaveChangesAsync();
    }
}
