using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Data;

public class DatabaseSeeder
{
    private readonly IGameSettingsRepository _gameSettingsRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;

    public DatabaseSeeder(
        IGameSettingsRepository gameSettingsRepository,
        IRoomRepository roomRepository,
        IUserRepository userRepository
    )
    {
        _gameSettingsRepository = gameSettingsRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
    }

    //Seed database with initial data
    public async Task Seed()
    {
        await EnsureAdminUserExists();
        await EnsureFirstRoomExists();
        await EnsureGameSettingsExist();
    }

    async Task EnsureAdminUserExists()
    {
        var adminInDb = await _userRepository.FindUser("admin");
        if (adminInDb is null)
        {
            string adminPassword = 
                Environment.GetEnvironmentVariable("MUS_256-BIT_KEY");

            var adminUser = new User()
            {
                IsBuilder = true,
                Username = "admin",
                HashedPassword = User.HashPassword(adminPassword)
            };
            
            await _userRepository.CreateUser(adminUser);
        }
    }

    async Task EnsureFirstRoomExists()
    {
        var roomInDb = await _roomRepository.GetFirstRoom();
        if (roomInDb is null)
        {
            var roomOne = new Room()
            {
                Name = "r1",
                Description = "Everything starts from one.",
                GlobalAccess = false,
                Inventory = new Inventory(),
                InBeing = null
            };
            await _roomRepository.CreateRoom(roomOne);
        }
    }

    async Task EnsureGameSettingsExist()
    {
        var settingsInDb = await _gameSettingsRepository.GetGameSettings();
        if (settingsInDb is null)
        {
            var firstRoom = await _roomRepository.GetFirstRoom();
            var gameSettings = new GameSettings()
            {
                DefaultSpawnRoom = firstRoom!
            };

            await _gameSettingsRepository.SetGameSettings(gameSettings);
        }
    }
}
