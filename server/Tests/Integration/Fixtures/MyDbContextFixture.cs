using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Integration.Fixtures;

public sealed class MyDbContextFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("Incident_Tracker")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    public async ValueTask DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public MyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        var context = new MyDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}


// Share the container across all tests in this collection
[CollectionDefinition("Repository")]
public class DatabaseCollection : ICollectionFixture<MyDbContextFixture>;

