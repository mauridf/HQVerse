using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using HQVerse.Application.DependencyInjection;
using HQVerse.CrossCutting.Extensions;
using HQVerse.Infrastructure.Data.Migrations;
using HQVerse.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
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

    // OpenAPI para .NET 10
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

    // Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Política global: 100 requisições por minuto por IP
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: clientIp,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                });
        });

        // Política específica para autenticação: 5 tentativas por minuto
        options.AddFixedWindowLimiter("AuthPolicy", config =>
        {
            config.PermitLimit = 5;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            config.QueueLimit = 2;
        });

        // Política para endpoints públicos: 30 requisições por minuto
        options.AddFixedWindowLimiter("PublicPolicy", config =>
        {
            config.PermitLimit = 30;
            config.Window = TimeSpan.FromMinutes(1);
        });
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevelopmentPolicy", builder =>
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("X-Total-Count", "X-Correlation-Id"));

        options.AddPolicy("ProductionPolicy", builder =>
            builder.WithOrigins(
                    "https://hqverse.vercel.app",
                    "https://hqverse.netlify.app",
                    "https://hqverse-app.onrender.com")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()
                   .WithExposedHeaders("X-Total-Count", "X-Correlation-Id")
                   .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));
    });

    // HTTP Logging (observabilidade)
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
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

    // Mapear OpenAPI endpoint
    app.MapOpenApi();

    // Scalar UI
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

    // Use custom middlewares (ordem importa!)
    app.UseCorrelationId();
    app.UseRequestLogging();
    app.UseGlobalExceptionHandler();

    // HTTP Logging
    app.UseHttpLogging();

    // Rate Limiter
    app.UseRateLimiter();

    // CORS - usar política de desenvolvimento ou produção
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("DevelopmentPolicy");
    }
    else
    {
        app.UseCors("ProductionPolicy");
    }

    // HTTPS Redirection (em produção)
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
        app.UseHsts();
    }

    // Security Headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
        await next();
    });

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