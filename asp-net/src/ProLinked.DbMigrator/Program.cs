using Microsoft.Extensions.DependencyInjection;
using ProLinked.DbMigrator.Data;

namespace ProLinked.DbMigrator;

public class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var startup = new Startup();
        startup.ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        var dbMigrationService = serviceProvider.GetService<ProLinkedDbMigrationService>() ?? throw new ("Service not found");
        await dbMigrationService.MigrateAsync();

        var dbSeedingService = serviceProvider.GetService<ProLinkedDbSeedingService>() ?? throw new ("Service not found");
        await dbSeedingService.SeedAsync();
        Console.WriteLine("You can press any button to exit...");
        Console.ReadLine();
    }
}