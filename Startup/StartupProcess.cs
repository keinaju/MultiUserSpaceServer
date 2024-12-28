using Microsoft.EntityFrameworkCore;
using MUS.Game.Data;

namespace MUS.Startup;

public class StartupProcess
{
    public IConfiguration Configuration { get; }

    public StartupProcess(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "MUS_MYSQL_CONNECTION_STRING"
        );
        var serverVersion = new MySqlServerVersion(new Version(8, 4, 3));

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
        });

        services.AddDbContext<GameContext>(dbContextOptions =>
            dbContextOptions.UseMySql(connectionString, serverVersion)
        );

        services.AddGameServices();

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.EnsureDatabaseExists();
    }
}
