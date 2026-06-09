using System.Reflection;
using System.Text;
using HQVerse.Application.DependencyInjection;
using HQVerse.CrossCutting.Extensions;
using HQVerse.Infrastructure.Data.Migrations;
using HQVerse.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
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

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // OpenAPI para .NET 10 (nativo, sem Swashbuckle)
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info.Title = "HQVerse API";
            document.Info.Version = "v1";
            document.Info.Description = @"API para gestão de Scans de HQs - Comunidade de leitores de HQs.

                ## Funcionalidades
                - **Editorial**: Publishers, Characters, Teams, ComicSeries, ComicIssues, StoryArcs
                - **Comunidade**: Usuários, Coleções, Reviews, Comentários, Favoritos
                - **Scans**: ScanGroups, Scans, Links de download/leitura
                - **Integração**: Comic Vine API para importação de metadados

                ## Autenticação
                A API usa JWT Bearer Token. Faça login em `/api/auth/login` e use o token no header:
                `Authorization: Bearer {seu-token}`";

            document.Info.Contact = new()
            {
                Name = "HQVerse Team",
                Email = "contact@hqverse.dev"
            };

            return Task.CompletedTask;
        });
    });

    // Infrastructure
    builder.Services.AddInfrastructure(builder.Configuration);

    // Application
    builder.Services.AddApplication();

    // JWT Authentication
    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("JWT Secret not configured.");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "HQVerse",
            ValidAudience = "HQVerse",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Mapear OpenAPI endpoint (necessário para Scalar)
    app.MapOpenApi();

    // Scalar UI - acessível em /scalar
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("HQVerse API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDarkModeToggle(true)
            .WithSidebar(true)
            .WithDotNetFlag(false);
    });

    // Redirecionar raiz para Scalar
    app.MapGet("/", () => Results.Redirect("/scalar"));

    // Use custom middlewares
    app.UseCorrelationId();
    app.UseRequestLogging();
    app.UseGlobalExceptionHandler();

    // Run database migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        dbInitializer.RunMigrations();
    }

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Map controllers
    app.MapControllers();

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