using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HQVerse.Infrastructure.Data.Migrations;

public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    public void RunMigrations()
    {
        _logger.LogInformation("Starting database migrations...");
        _logger.LogInformation("Connection string found (first 20 chars): {ConnectionStart}",
            _connectionString.Substring(0, Math.Min(20, _connectionString.Length)));

        // Para Render, pulamos EnsureDatabaseExists pois o banco já existe
        // O DbUp criará as tabelas automaticamente

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Database migration failed!");
            throw result.Error;
        }

        _logger.LogInformation("Database migrations completed successfully!");
    }
}