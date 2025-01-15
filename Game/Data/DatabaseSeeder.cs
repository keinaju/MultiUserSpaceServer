using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data;

public class DatabaseSeeder
{
    private const string ADMIN_USERNAME = "admin";
    private const string SECRET_ENVIRONMENT_VARIABLE_NAME = "MUS_256-BIT_KEY";

    private readonly GameContext _context;

    public DatabaseSeeder(GameContext context)
    {
        _context = context;
    }

    //Seed database with initial data
    public async Task Seed()
    {
        await EnsureFirstRoomExists();
        await EnsureAdminUserExists();
        await EnsureGameSettingsExist();
    }

    async Task EnsureAdminUserExists()
    {
        var adminUser = await _context.FindUser(username: ADMIN_USERNAME);
        if(adminUser is null)
        {
            var secret = Environment.GetEnvironmentVariable(SECRET_ENVIRONMENT_VARIABLE_NAME);
            if(secret is null)
            {
                throw new Exception($"Environment variable {SECRET_ENVIRONMENT_VARIABLE_NAME} is not defined.");
            }

            var newUser = new User()
            {
                IsAdmin = true,
                Username = ADMIN_USERNAME,
                HashedPassword = User.HashPassword(secret)
            };

            await _context.Users.AddAsync(newUser);

            var newBeing = new Being()
            {   
                CreatedByUser = newUser,
                FreeInventory = new Inventory(),
                InRoom = await _context.Rooms.FirstAsync(),
                Name = "ALAN",
                TradeInventory = new Inventory()
            };

            newUser.CreatedBeings.Add(newBeing);

            await _context.SaveChangesAsync();
        }
    }

    async Task EnsureFirstRoomExists()
    {
        var firstRoom = await _context.Rooms.FirstOrDefaultAsync();

        if (firstRoom is null)
        {
            await _context.Rooms.AddAsync(
                new Room()
                {
                    ConnectionLimit = 4,
                    Name = "MAIN",
                    Description = "This is the default room of the application.",
                    GlobalAccess = false,
                    Inventory = new Inventory(),
                    InBeing = null
                }
            );
            
            await _context.SaveChangesAsync();
        }
    }

    async Task EnsureGameSettingsExist()
    {
        var settings = await _context.GetGameSettings();

        if (settings is null)
        {
            var firstRoom = await _context.Rooms.FirstAsync();

            await _context.GameSettings.AddAsync(
                new GameSettings()
                {
                    DefaultSpawnRoom = firstRoom,
                    GameName = "TEX Online -application",
                    GameDescription = null,
                    TickIntervalSeconds = 10
                }
            );

            await _context.SaveChangesAsync();
        }
    }
}
