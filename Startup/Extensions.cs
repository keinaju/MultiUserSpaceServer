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
        services.AddScoped<ICommandProvider, CommandProvider>();
        services.AddScoped<ICommandParser, CommandParser>();
        
        services.AddScoped<ICommandPattern, BeingHasFeatureCommand>();
        services.AddScoped<ICommandPattern, BeingNameIsCommand>();
        services.AddScoped<ICommandPattern, BreakCommand>();
        services.AddScoped<ICommandPattern, CraftCommand>();
        services.AddScoped<ICommandPattern, CuriosityIsCommand>();
        services.AddScoped<ICommandPattern, DeleteBeingCommand>();
        services.AddScoped<ICommandPattern, DeleteFeatureCommand>();
        services.AddScoped<ICommandPattern, DeleteItemCommand>();
        services.AddScoped<ICommandPattern, DeleteRoomCommand>();
        services.AddScoped<ICommandPattern, DeleteRoomPoolCommand>();
        services.AddScoped<ICommandPattern, DeployCommand>();
        services.AddScoped<ICommandPattern, DeploymentIsCommand>();
        services.AddScoped<ICommandPattern, ExploreCommand>();
        services.AddScoped<ICommandPattern, FeatureNameIsCommand>();
        services.AddScoped<ICommandPattern, GameNameIsCommand>();
        services.AddScoped<ICommandPattern, GoCommand>();
        services.AddScoped<ICommandPattern, HelpCommand>();
        services.AddScoped<ICommandPattern, ItemDescriptionIsCommand>();
        services.AddScoped<ICommandPattern, ItemHatcherIntervalIsCommand>();
        services.AddScoped<ICommandPattern, ItemHatcherQuantityIsCommand>();
        services.AddScoped<ICommandPattern, ItemIsMadeOfCommand>();
        services.AddScoped<ICommandPattern, ItemNameIsCommand>();
        services.AddScoped<ICommandPattern, MakeItemsCommand>();
        services.AddScoped<ICommandPattern, NewBeingCommand>();
        services.AddScoped<ICommandPattern, NewFeatureCommand>();
        services.AddScoped<ICommandPattern, NewItemHatcherCommand>();
        services.AddScoped<ICommandPattern, NewItemCommand>();
        services.AddScoped<ICommandPattern, NewRoomCommand>();
        services.AddScoped<ICommandPattern, NewRoomPoolCommand>();
        services.AddScoped<ICommandPattern, PingCommand>();
        services.AddScoped<ICommandPattern, RoomDescriptionIsCommand>();
        services.AddScoped<ICommandPattern, RoomHasRequirementCommand>();
        services.AddScoped<ICommandPattern, RoomIsGlobalCommand>();
        services.AddScoped<ICommandPattern, RoomIsInRoomPoolCommand>();
        services.AddScoped<ICommandPattern, RoomIsInsideCommand>();
        services.AddScoped<ICommandPattern, RoomNameIsCommand>();
        services.AddScoped<ICommandPattern, RoomPoolDescriptionIsCommand>();
        services.AddScoped<ICommandPattern, RoomPoolFeeIsCommand>();
        services.AddScoped<ICommandPattern, RoomPoolNameIsCommand>();
        services.AddScoped<ICommandPattern, SelectBeingCommand>();
        services.AddScoped<ICommandPattern, ShowBeingCommand>();
        services.AddScoped<ICommandPattern, ShowFeaturesCommand>();
        services.AddScoped<ICommandPattern, ShowGlobalRoomsCommand>();
        services.AddScoped<ICommandPattern, ShowInventoryCommand>();
        services.AddScoped<ICommandPattern, ShowItemCommand>();
        services.AddScoped<ICommandPattern, ShowItemsCommand>();
        services.AddScoped<ICommandPattern, ShowRoomCommand>();
        services.AddScoped<ICommandPattern, ShowRoomPoolCommand>();
        services.AddScoped<ICommandPattern, ShowRoomPoolsCommand>();
        services.AddScoped<ICommandPattern, ShowTimeCommand>();
        services.AddScoped<ICommandPattern, ShowUserCommand>();
        services.AddScoped<ICommandPattern, SignInCommand>();
        services.AddScoped<ICommandPattern, SignUpCommand>();
        services.AddScoped<ICommandPattern, TakeCommand>();
        services.AddScoped<ICommandPattern, TickIntervalIsCommand>();
        services.AddScoped<ICommandPattern, TradeCommand>();
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
        services.AddScoped<IUserSession, UserSession>();
        
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
