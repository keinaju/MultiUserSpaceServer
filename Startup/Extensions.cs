using Microsoft.EntityFrameworkCore;
using MUS.Game;
using MUS.Game.Clock;
using MUS.Game.Commands;
using MUS.Game.Commands.Delete;
using MUS.Game.Commands.Generic;
using MUS.Game.Commands.Is;
using MUS.Game.Commands.Make;
using MUS.Game.Commands.New;
using MUS.Game.Commands.Show;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Startup;

public static class Extensions
{
    private static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<GameClock>();
        services.AddScoped<IGameClockListener, TickSaveListener>();
        services.AddScoped<IGameClockListener, ItemHatcherListener>();
    }

    private static void AddCommands(this IServiceCollection services)
    {
        services.AddScoped<ICommandParser, CommandParser>();

        services.AddScoped<IGameCommand, BeingIsFeatureCommand>();
        services.AddScoped<IGameCommand, BeingNameIsCommand>();
        services.AddScoped<IGameCommand, BreakCommand>();
        services.AddScoped<IGameCommand, CraftCommand>();
        services.AddScoped<IGameCommand, CuriosityIsCommand>();
        services.AddScoped<IGameCommand, DeleteBeingCommand>();
        services.AddScoped<IGameCommand, DeleteFeatureCommand>();
        services.AddScoped<IGameCommand, DeleteItemCommand>();
        services.AddScoped<IGameCommand, DeleteRoomCommand>();
        services.AddScoped<IGameCommand, DeleteRoomPoolCommand>();
        services.AddScoped<IGameCommand, DeployCommand>();
        services.AddScoped<IGameCommand, DeploymentIsCommand>();
        services.AddScoped<IGameCommand, ExploreCommand>();
        services.AddScoped<IGameCommand, FeatureNameIsCommand>();
        services.AddScoped<IGameCommand, GoCommand>();
        services.AddScoped<IGameCommand, ItemDescriptionIsCommand>();
        services.AddScoped<IGameCommand, ItemHatcherIntervalIsCommand>();
        services.AddScoped<IGameCommand, ItemHatcherQuantityIsCommand>();
        services.AddScoped<IGameCommand, ItemIsMadeOfCommand>();
        services.AddScoped<IGameCommand, ItemNameIsCommand>();
        services.AddScoped<IGameCommand, MakeItemsCommand>();
        services.AddScoped<IGameCommand, NewBeingCommand>();
        services.AddScoped<IGameCommand, NewFeatureCommand>();
        services.AddScoped<IGameCommand, NewItemHatcherCommand>();
        services.AddScoped<IGameCommand, NewItemCommand>();
        services.AddScoped<IGameCommand, NewRoomCommand>();
        services.AddScoped<IGameCommand, NewRoomPoolCommand>();
        services.AddScoped<IGameCommand, PingCommand>();
        services.AddScoped<IGameCommand, RoomDescriptionIsCommand>();
        services.AddScoped<IGameCommand, RoomIsForCommand>();
        services.AddScoped<IGameCommand, RoomIsGlobalCommand>();
        services.AddScoped<IGameCommand, RoomIsInBeingCommand>();
        services.AddScoped<IGameCommand, RoomIsInRoomPoolCommand>();
        services.AddScoped<IGameCommand, RoomNameIsCommand>();
        services.AddScoped<IGameCommand, RoomPoolDescriptionIsCommand>();
        services.AddScoped<IGameCommand, RoomPoolFeeIsCommand>();
        services.AddScoped<IGameCommand, RoomPoolNameIsCommand>();
        services.AddScoped<IGameCommand, SelectBeingCommand>();
        services.AddScoped<IGameCommand, SellCommand>();
        services.AddScoped<IGameCommand, ShowBeingCommand>();
        services.AddScoped<IGameCommand, ShowCommandsCommand>();
        services.AddScoped<IGameCommand, ShowFeaturesCommand>();
        services.AddScoped<IGameCommand, ShowGlobalRoomsCommand>();
        services.AddScoped<IGameCommand, ShowInventoryCommand>();
        services.AddScoped<IGameCommand, ShowItemCommand>();
        services.AddScoped<IGameCommand, ShowItemsCommand>();
        services.AddScoped<IGameCommand, ShowOffersCommand>();
        services.AddScoped<IGameCommand, ShowRoomCommand>();
        services.AddScoped<IGameCommand, ShowRoomPoolCommand>();
        services.AddScoped<IGameCommand, ShowRoomPoolsCommand>();
        services.AddScoped<IGameCommand, ShowTimeCommand>();
        services.AddScoped<IGameCommand, ShowUserCommand>();
        services.AddScoped<IGameCommand, SignInCommand>();
        services.AddScoped<IGameCommand, SignUpCommand>();
        services.AddScoped<IGameCommand, TakeCommand>();
        services.AddScoped<IGameCommand, TickIntervalIsCommand>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBeingRepository, BeingRepository>();
        services.AddScoped<ICraftPlanRepository, CraftPlanRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IGameSettingsRepository, GameSettingsRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IItemHatcherRepository, ItemHatcherRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IItemStackRepository, ItemStackRepository>();
        services.AddScoped<IPlayerState, PlayerState>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomPoolRepository, RoomPoolRepository>();
        services.AddScoped<ITickCounterRepository, TickCounterRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddRequestHandling(this IServiceCollection services)
    {
        services.AddScoped<IResponsePayload, ResponsePayload>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IInputCommand, InputCommand>();
    }

    private static void AddSessions(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        
        services.AddTransient<ITokenService, TokenService>();
    }

    private static void AddUtilities(this IServiceCollection services)
    {
        services.AddSingleton<IGameUptime, GameUptime>();
    }

    public static void AddGameServices(this IServiceCollection services)
    {
        services.AddBackgroundServices();
        services.AddCommands();
        services.AddRepositories();
        services.AddRequestHandling();
        services.AddSessions();
        services.AddUtilities();
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
                userRepository: provider.GetRequiredService<IUserRepository>()
            )
            .Seed()
            .Wait();
        }
    }
}
