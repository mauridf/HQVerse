using HQVerse.Infrastructure.Data.Migrations;
using HQVerse.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting HQVerse API...");

    // Add services
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Run database migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        dbInitializer.RunMigrations();
    }

    app.MapGet("/", () => "HQVerse API is running!");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}