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
        services.AddScoped<ICommandCollection, CommandCollection>();
        services.AddScoped<ICommandParser, CommandParser>();
        
        services.AddScoped<IUserCommand, BeingHasFeatureCommand>();
        services.AddScoped<IUserCommand, BeingNameIsCommand>();
        services.AddScoped<IUserCommand, BreakCommand>();
        services.AddScoped<IUserCommand, CraftCommand>();
        services.AddScoped<IUserCommand, CuriosityIsCommand>();
        services.AddScoped<IUserCommand, DeleteBeingCommand>();
        services.AddScoped<IUserCommand, DeleteFeatureCommand>();
        services.AddScoped<IUserCommand, DeleteItemCommand>();
        services.AddScoped<IUserCommand, DeleteRoomCommand>();
        services.AddScoped<IUserCommand, DeleteRoomPoolCommand>();
        services.AddScoped<IUserCommand, DeployCommand>();
        services.AddScoped<IUserCommand, DeploymentIsCommand>();
        services.AddScoped<IUserCommand, ExploreCommand>();
        services.AddScoped<IUserCommand, FeatureNameIsCommand>();
        services.AddScoped<IUserCommand, GameNameIsCommand>();
        services.AddScoped<IUserCommand, GoCommand>();
        services.AddScoped<IUserCommand, HelpCommand>();
        services.AddScoped<IUserCommand, ItemDescriptionIsCommand>();
        services.AddScoped<IUserCommand, ItemHatcherIntervalIsCommand>();
        services.AddScoped<IUserCommand, ItemHatcherQuantityIsCommand>();
        services.AddScoped<IUserCommand, ItemIsMadeOfCommand>();
        services.AddScoped<IUserCommand, ItemNameIsCommand>();
        services.AddScoped<IUserCommand, MakeItemsCommand>();
        services.AddScoped<IUserCommand, NewBeingCommand>();
        services.AddScoped<IUserCommand, NewFeatureCommand>();
        services.AddScoped<IUserCommand, NewItemHatcherCommand>();
        services.AddScoped<IUserCommand, NewItemCommand>();
        services.AddScoped<IUserCommand, NewRoomCommand>();
        services.AddScoped<IUserCommand, NewRoomPoolCommand>();
        services.AddScoped<IUserCommand, PingCommand>();
        services.AddScoped<IUserCommand, RoomDescriptionIsCommand>();
        services.AddScoped<IUserCommand, RoomHasRequirementCommand>();
        services.AddScoped<IUserCommand, RoomIsGlobalCommand>();
        services.AddScoped<IUserCommand, RoomIsInRoomPoolCommand>();
        services.AddScoped<IUserCommand, RoomIsInsideCommand>();
        services.AddScoped<IUserCommand, RoomNameIsCommand>();
        services.AddScoped<IUserCommand, RoomPoolDescriptionIsCommand>();
        services.AddScoped<IUserCommand, RoomPoolFeeIsCommand>();
        services.AddScoped<IUserCommand, RoomPoolNameIsCommand>();
        services.AddScoped<IUserCommand, SelectBeingCommand>();
        services.AddScoped<IUserCommand, ShowBeingCommand>();
        services.AddScoped<IUserCommand, ShowFeaturesCommand>();
        services.AddScoped<IUserCommand, ShowGlobalRoomsCommand>();
        services.AddScoped<IUserCommand, ShowInventoryCommand>();
        services.AddScoped<IUserCommand, ShowItemCommand>();
        services.AddScoped<IUserCommand, ShowItemsCommand>();
        services.AddScoped<IUserCommand, ShowOffersCommand>();
        services.AddScoped<IUserCommand, ShowRoomCommand>();
        services.AddScoped<IUserCommand, ShowRoomPoolCommand>();
        services.AddScoped<IUserCommand, ShowRoomPoolsCommand>();
        services.AddScoped<IUserCommand, ShowTimeCommand>();
        services.AddScoped<IUserCommand, ShowUserCommand>();
        services.AddScoped<IUserCommand, SignInCommand>();
        services.AddScoped<IUserCommand, SignUpCommand>();
        services.AddScoped<IUserCommand, TakeCommand>();
        services.AddScoped<IUserCommand, TickIntervalIsCommand>();
        services.AddScoped<IUserCommand, TradeCommand>();
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
