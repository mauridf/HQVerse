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

        // Cria o banco manualmente se não existir (evita dependência de EnsureDatabase extension)
        using (var connection = new Npgsql.NpgsqlConnection(masterConnectionString))
        {
            connection.Open();

            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @name";
                var param = checkCmd.CreateParameter();
                param.ParameterName = "name";
                param.Value = databaseName;
                checkCmd.Parameters.Add(param);

                var exists = checkCmd.ExecuteScalar() != null;

                if (!exists)
                {
                    using (var createCmd = connection.CreateCommand())
                    {
                        // Nome do banco pode precisar ser escapado; aqui usamos identificador entre aspas
                        createCmd.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                        createCmd.ExecuteNonQuery();
                    }

                    _logger.LogInformation("Database '{DatabaseName}' created.", databaseName);
                }
                else
                {
                    _logger.LogInformation("Database '{DatabaseName}' already exists.", databaseName);
                }
            }
        }
    }
}