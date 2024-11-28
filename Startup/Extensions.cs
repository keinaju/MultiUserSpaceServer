using Microsoft.EntityFrameworkCore;
using MUS.Game;
using MUS.Game.Clock;
using MUS.Game.Commands;
using MUS.Game.Commands.Rename;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;

namespace MUS.Startup;

public static class Extensions
{
    private static void AddCommands(this IServiceCollection services)
    {
        services.AddScoped<ICommandParser, CommandParser>();
        services.AddScoped<IGameCommand, AddRoomInRoomPoolCommand>();
        services.AddScoped<IGameCommand, EmptyStringCommand>();
        services.AddScoped<IGameCommand, ExploreCommand>();
        services.AddScoped<IGameCommand, GetCommand>();
        services.AddScoped<IGameCommand, GoToCommand>();
        services.AddScoped<IGameCommand, HelpCommand>();
        services.AddScoped<IGameCommand, NewBeingCommand>();
        services.AddScoped<IGameCommand, NewItemGeneratorCommand>();
        services.AddScoped<IGameCommand, NewItemCommand>();
        services.AddScoped<IGameCommand, NewRoomCommand>();
        services.AddScoped<IGameCommand, NewRoomPoolCommand>();
        services.AddScoped<IGameCommand, NewCuriosityCommand>();
        services.AddScoped<IGameCommand, LoginCommand>();
        services.AddScoped<IGameCommand, LookCommand>();
        services.AddScoped<IGameCommand, MyCommand>();
        services.AddScoped<IGameCommand, PickBeingCommand>();
        services.AddScoped<IGameCommand, RenameItemCommand>();
        services.AddScoped<IGameCommand, RenameRoomCommand>();
        services.AddScoped<IGameCommand, SignupCommand>();
        services.AddScoped<IGameCommand, TimeCommand>();
        services.AddScoped<IGameCommand, UserCommand>();
        services.AddScoped<IPrerequisiteFilter, PrerequisiteFilter>();
    }

    private static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<GameClock>();
        services.AddScoped<IGameClockListener, TickSaveListener>();
        services.AddScoped<IGameClockListener, ItemGeneratorListener>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBeingRepository, BeingRepository>();
        services.AddScoped<IGameSettingsRepository, GameSettingsRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IItemGeneratorRepository, ItemGeneratorRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IItemStackRepository, ItemStackRepository>();
        services.AddScoped<ICuriosityRepository, CuriosityRepository>();
        services.AddScoped<IPlayerState, PlayerState>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomPoolRepository, RoomPoolRepository>();
        services.AddScoped<ITickCounterRepository, TickCounterRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddRequestHandling(this IServiceCollection services)
    {
        services.AddScoped<IGameService, GameService>();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
    }

    private static void AddSessions(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        services.AddTransient<ITokenService, TokenService>();
    }

    public static void AddGameServices(this IServiceCollection services)
    {
        services.AddCommands();
        services.AddRepositories();
        services.AddSessions();
        services.AddRequestHandling();
        services.AddBackgroundServices();
    }

    public static void EnsureDatabaseExists(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<GameContext>();
            context.Database.Migrate();

            new DatabaseSeeder(
                gameSettingsRepository: provider.GetRequiredService<IGameSettingsRepository>(),
                roomRepository: provider.GetRequiredService<IRoomRepository>(),
                tickCounterRepository: provider.GetRequiredService<ITickCounterRepository>(),
                userRepository: provider.GetRequiredService<IUserRepository>()
            )
            .Seed()
            .Wait();
        }
    }
}
