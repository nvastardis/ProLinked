using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProLinked.Infrastructure.Data;

namespace ProLinked.TestBase;

public class DbContextFactory: IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection? _sqliteConnection = null;

    public async Task<ProLinkedDbContext> CreateSqliteAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ProLinkedDbContext>()
            .UseSqlite()
            .Options;

        var context = new ProLinkedDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        return context;

    }

    public void Dispose()
    {
        _sqliteConnection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_sqliteConnection != null)
        {
            await _sqliteConnection.DisposeAsync();
        }
    }
}