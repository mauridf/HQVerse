using System.Reflection;
using DbUp;
using DbUp.Engine;
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

        EnsureDatabaseExists();

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

    private void EnsureDatabaseExists()
    {
        // Extrai as partes da connection string
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(_connectionString);
        var databaseName = builder.Database;

        // Cria connection string para o banco 'postgres' (banco padrão)
        builder.Database = "postgres";
        var masterConnectionString = builder.ConnectionString;

        // Usa a API correta do DbUp para PostgreSQL
        DbUp.EnsureDatabase.For.PostgresqlDatabase(masterConnectionString, databaseName);

        _logger.LogInformation("Database '{DatabaseName}' verified/created.", databaseName);
    }
}