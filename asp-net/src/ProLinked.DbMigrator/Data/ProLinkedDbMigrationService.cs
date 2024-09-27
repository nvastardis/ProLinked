using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProLinked.Infrastructure.Data;

namespace ProLinked.DbMigrator.Data;

public class ProLinkedDbMigrationService
{
    private readonly ProLinkedDbContext _dbContext;
    private readonly ILogger Logger;


    public ProLinkedDbMigrationService(
        ILogger<ProLinkedDbMigrationService> logger,
        ProLinkedDbContext dbContext)
    {
        Logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> MigrateAsync()
    {
        Logger.LogInformation("Started database migrations...");
        if (!await _dbContext.Database.CanConnectAsync())
        {
            Logger.LogInformation("Creating initial migration...");
            await _dbContext.Database.MigrateAsync();
            return false;
        }

        if (!(await _dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            Logger.LogInformation("Database already up to date.");
        }

        Logger.LogInformation("Migrating schema for host database...");
        await _dbContext.Database.MigrateAsync();
        Logger.LogInformation($"Successfully completed host database migrations.");
        return true;
    }
}