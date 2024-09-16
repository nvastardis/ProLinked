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

    public async Task MigrateAsync()
    {
        await AddInitialMigrationIfNotExist();
        Logger.LogInformation("Started database migrations...");

        await MigrateDatabaseSchemaAsync();
        Logger.LogInformation($"Successfully completed host database migrations.");
    }

    private async Task MigrateDatabaseSchemaAsync()
    {
        Logger.LogInformation(
            $"Migrating schema for host database...");

        await _dbContext.Database.MigrateAsync();
    }

    private async Task<bool> AddInitialMigrationIfNotExist()
    {
        try
        {
            if (MigrationsFolderExists())
            {
                return false;
            }
            await AddInitialMigration();
            return true;

        }
        catch (Exception e)
        {
            Logger.LogWarning("Couldn't determinate if any migrations exist : " + e.Message);
            return false;
        }
    }

    private bool MigrationsFolderExists()
    {
        var dbMigrationsProjectFolder = GetEntityFrameworkCoreProjectFolderPath();
        return dbMigrationsProjectFolder != null && Directory.Exists(Path.Combine(dbMigrationsProjectFolder, "Migrations"));
    }

    private async Task AddInitialMigration()
    {
        Logger.LogInformation("Creating initial migration...");

        await _dbContext.Database.MigrateAsync();
    }

    private string? GetEntityFrameworkCoreProjectFolderPath()
    {
        var slnDirectoryPath = GetSolutionDirectoryPath();

        if (slnDirectoryPath == null)
        {
            throw new Exception("Solution folder not found!");
        }

        var srcDirectoryPath = Path.Combine(slnDirectoryPath, "src");

        return Directory.GetDirectories(srcDirectoryPath)
            .FirstOrDefault(d => d.EndsWith(".Infrastructure"));
    }

    private string? GetSolutionDirectoryPath()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDirectory != null && Directory.GetParent(currentDirectory.FullName) != null)
        {
            currentDirectory = Directory.GetParent(currentDirectory.FullName);

            if (currentDirectory != null && Directory.GetFiles(currentDirectory.FullName).FirstOrDefault(f => f.EndsWith(".sln")) != null)
            {
                return currentDirectory.FullName;
            }
        }

        return null;
    }
}