using Microsoft.EntityFrameworkCore;
using MUS.Game;
using MUS.Game.Clock;
using MUS.Game.Commands;
using MUS.Game.Commands.Grant;
using MUS.Game.Commands.New;
using MUS.Game.Commands.Set;
using MUS.Game.Commands.Show;
using MUS.Game.Data;
using MUS.Game.Data.Repositories;
using MUS.Game.Session;
using MUS.Game.Utilities;

namespace MUS.Startup;

public static class Extensions
{
    private static void AddCommands(this IServiceCollection services)
    {
        services.AddScoped<ICommandParser, CommandParser>();
        services.AddScoped<IGameCommand, AddFeatureInBeingCommand>();
        services.AddScoped<IGameCommand, AddFeatureInRoomCommand>();
        services.AddScoped<IGameCommand, AddItemInCraftPlanCommand>();
        services.AddScoped<IGameCommand, AddRoomInRoomPoolCommand>();
        services.AddScoped<IGameCommand, BreakOrCraftCommand>();
        services.AddScoped<IGameCommand, DeployCommand>();
        services.AddScoped<IGameCommand, ExploreCommand>();
        services.AddScoped<IGameCommand, GoCommand>();
        services.AddScoped<IGameCommand, LeaveCommand>();
        services.AddScoped<IGameCommand, GrantItemCommand>();
        services.AddScoped<IGameCommand, IntroductionCommand>();
        services.AddScoped<IGameCommand, LoginCommand>();
        services.AddScoped<IGameCommand, LookCommand>();
        services.AddScoped<IGameCommand, NewBeingCommand>();
        services.AddScoped<IGameCommand, NewCraftPlanCommand>();
        services.AddScoped<IGameCommand, NewDeploymentCommand>();
        services.AddScoped<IGameCommand, NewFeatureCommand>();
        services.AddScoped<IGameCommand, NewItemHatcherCommand>();
        services.AddScoped<IGameCommand, NewItemCommand>();
        services.AddScoped<IGameCommand, NewRoomCommand>();
        services.AddScoped<IGameCommand, NewRoomPoolCommand>();
        services.AddScoped<IGameCommand, MyCommand>();
        services.AddScoped<IGameCommand, PingCommand>();
        services.AddScoped<IGameCommand, SelectBeingCommand>();
        services.AddScoped<IGameCommand, SellCommand>();
        services.AddScoped<IGameCommand, SetBeingNameCommand>();
        services.AddScoped<IGameCommand, SetCuriosityCommand>();
        services.AddScoped<IGameCommand, SetItemHatcherIntervalCommand>();
        services.AddScoped<IGameCommand, SetItemHatcherQuantityCommand>();
        services.AddScoped<IGameCommand, SetItemDescriptionCommand>();
        services.AddScoped<IGameCommand, SetItemNameCommand>();
        services.AddScoped<IGameCommand, SetRoomDescriptionCommand>();
        services.AddScoped<IGameCommand, SetRoomGlobalAccessCommand>();
        services.AddScoped<IGameCommand, SetRoomInsideBeingCommand>();
        services.AddScoped<IGameCommand, SetRoomNameCommand>();
        services.AddScoped<IGameCommand, SetRoomPoolDescriptionCommand>();
        services.AddScoped<IGameCommand, SetRoomPoolNameCommand>();
        services.AddScoped<IGameCommand, SetRoomPoolRequiredItemCommand>();
        services.AddScoped<IGameCommand, ShowCommandsCommand>();
        services.AddScoped<IGameCommand, ShowFeaturesCommand>();
        services.AddScoped<IGameCommand, ShowGlobalRoomsCommand>();
        services.AddScoped<IGameCommand, ShowItemCommand>();
        services.AddScoped<IGameCommand, ShowItemsCommand>();
        services.AddScoped<IGameCommand, ShowItemHatchersCommand>();
        services.AddScoped<IGameCommand, ShowOffersCommand>();
        services.AddScoped<IGameCommand, ShowRoomPoolsCommand>();
        services.AddScoped<IGameCommand, ShowUserCommand>();
        services.AddScoped<IGameCommand, SignupCommand>();
        services.AddScoped<IGameCommand, TakeCommand>();
        services.AddScoped<IGameCommand, TimeCommand>();
        services.AddScoped<IPrerequisiteFilter, PrerequisiteFilter>();
    }

    private static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<GameClock>();
        services.AddScoped<IGameClockListener, TickSaveListener>();
        services.AddScoped<IGameClockListener, ItemHatcherListener>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBeingRepository, BeingRepository>();
        services.AddScoped<ICraftPlanRepository, CraftPlanRepository>();
        services.AddScoped<IDeploymentRepository, DeploymentRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IGameSettingsRepository, GameSettingsRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IItemHatcherRepository, ItemHatcherRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IItemStackRepository, ItemStackRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IPlayerState, PlayerState>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomPoolRepository, RoomPoolRepository>();
        services.AddScoped<ITickCounterRepository, TickCounterRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddRequestHandling(this IServiceCollection services)
    {
        services.AddScoped<IGameResponse, GameResponse>();
        services.AddScoped<IGameService, GameService>();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
    }

    private static void AddSessions(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        services.AddTransient<ITokenService, TokenService>();
    }

    private static void AddUtilities(this IServiceCollection services)
    {
        services.AddScoped<IOfferManager, OfferManager>();
    }

    public static void AddGameServices(this IServiceCollection services)
    {
        services.AddCommands();
        services.AddRepositories();
        services.AddSessions();
        services.AddRequestHandling();
        services.AddBackgroundServices();
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
                tickCounterRepository: provider.GetRequiredService<ITickCounterRepository>(),
                userRepository: provider.GetRequiredService<IUserRepository>()
            )
            .Seed()
            .Wait();
        }
    }
}
