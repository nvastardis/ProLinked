using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProLinked.Infrastructure.Data;

public class ProLinkedDbContextFactory: IDesignTimeDbContextFactory<ProLinkedDbContext>
{
    public ProLinkedDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<ProLinkedDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new ProLinkedDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();

    }
}
