using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using ProLinked.DbMigrator.Data;
using ProLinked.Infrastructure;

namespace ProLinked.DbMigrator;

public class Startup
{
    public void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbConnection(
            configuration.GetConnectionString("Default") ?? throw new KeyNotFoundException()
        );
        serviceCollection.AddIdentity();
        serviceCollection.AddRepositories();
        serviceCollection.AddLogging(options =>
        {
            options.AddSimpleConsole(options =>
            {
                options.IncludeScopes = false;
            });
            options.AddConfiguration(configuration.GetSection("Logging"));
        });
        serviceCollection.AddTransient<ProLinkedDbMigrationService>();
        serviceCollection.AddTransient<ProLinkedDataSeeder>();
        serviceCollection.AddTransient<ProLinkedDbSeedingService>();
    }
}