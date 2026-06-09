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
        var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Converter URI postgres:// para formato padrão se necessário
        _connectionString = ConvertPostgresUriToConnectionString(rawConnectionString);
        _logger = logger;
    }

    public void RunMigrations()
    {
        _logger.LogInformation("Starting database migrations...");
        _logger.LogInformation("Connection string format: {Format}",
            _connectionString.Contains("Host=") ? "Standard" : "URI (converted)");

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

    /// <summary>
    /// Converte connection string no formato URI (postgres:// ou postgresql://) 
    /// para o formato padrão Host=xxx;Database=xxx;Username=xxx;Password=xxx
    /// </summary>
    private static string ConvertPostgresUriToConnectionString(string connectionString)
    {
        // Se já está no formato padrão, retorna como está
        if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        // Tenta converter URI para formato padrão
        try
        {
            if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(connectionString);

                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');
                var username = uri.UserInfo.Split(':')[0];
                var password = uri.UserInfo.Split(':').Length > 1 ? uri.UserInfo.Split(':')[1] : string.Empty;

                var converted = $"Host={host};Port={port};Database={database};Username={username};Password={password};Include Error Detail=true";

                return converted;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse connection string URI. Original: '{connectionString.Substring(0, Math.Min(30, connectionString.Length))}...'", ex);
        }

        return connectionString;
    }
}