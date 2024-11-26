using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    Task DeleteUser(int primaryKey);
    Task<User> FindUser(int primaryKey);
    Task<User?> FindUser(string username);
    Task UpdateUser(User updatedUser);
}
