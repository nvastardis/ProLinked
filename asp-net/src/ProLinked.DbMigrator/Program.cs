using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProLinked.DbMigrator.Data;

namespace ProLinked.DbMigrator;

public class Program
{
    public static async Task Main(string[] args)
    {
        var appBuilder = new HostApplicationBuilder();
        appBuilder.Environment.EnvironmentName = appBuilder.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT")!;
        appBuilder.
            Configuration.
            AddEnvironmentVariables().
            AddJsonFile($"appsettings.{appBuilder.Environment.EnvironmentName}.json", optional: true);

        Console.WriteLine($"Environment Settings: appsettings.{appBuilder.Environment.EnvironmentName}.json");
        var startup = new Startup();
        startup.ConfigureServices(appBuilder.Services, appBuilder.Configuration);
        var serviceProvider = appBuilder.Services.BuildServiceProvider();

        var dbMigrationService = serviceProvider.GetService<ProLinkedDbMigrationService>() ?? throw new ("Service not found");
        await dbMigrationService.MigrateAsync();

        var dbSeedingService = serviceProvider.GetService<ProLinkedDbSeedingService>() ?? throw new ("Service not found");
        await dbSeedingService.SeedAsync();
    }
}