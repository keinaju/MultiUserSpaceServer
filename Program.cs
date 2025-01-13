using MUS.Startup;

namespace MUS;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
            webBuilder => { webBuilder.UseStartup<StartupProcess>(); }
        );
    }
}
