using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Data;

public class DatabaseSeeder
{
    private readonly IGameSettingsRepository _gameSettingsRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ITickCounterRepository _tickCounterRepository;
    private readonly IUserRepository _userRepository;

    public DatabaseSeeder(
        IGameSettingsRepository gameSettingsRepository,
        IRoomRepository roomRepository,
        ITickCounterRepository tickCounterRepository,
        IUserRepository userRepository
    )
    {
        _gameSettingsRepository = gameSettingsRepository;
        _roomRepository = roomRepository;
        _tickCounterRepository = tickCounterRepository;
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
            string adminPassword = Environment.GetEnvironmentVariable("256-BIT_KEY");
            var adminUser = new User()
            {
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
                Name = "R-1",
                Description = "Everything starts from one.",
                Inventory = new Inventory()
            };
            await _roomRepository.CreateRoom(roomOne);
        }
    }

    async Task EnsureGameSettingsExist()
    {
        var settingsInDb = await _gameSettingsRepository.GetGameSettings();
        if (settingsInDb is null)
        {
            var gameSettings = new GameSettings()
            {
                DefaultSpawnRoom = await _roomRepository.GetFirstRoom(),
            };

            await _gameSettingsRepository.SetGameSettings(gameSettings);
        }
    }
}
