using HQVerse.Domain.Interfaces;
using HQVerse.Infrastructure.Data.Context;
using HQVerse.Infrastructure.Data.Migrations;
using HQVerse.Infrastructure.Data.Repositories;
using HQVerse.Infrastructure.ExternalServices.ComicVine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HQVerse.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<HQVerseDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        // Database Initializer (DbUp)
        services.AddTransient<DatabaseInitializer>();

        // Repositories
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<IComicSeriesRepository, ComicSeriesRepository>();
        services.AddScoped<IComicIssueRepository, ComicIssueRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Comic Vine Client
        services.AddHttpClient<ComicVineClient>(client =>
        {
            client.BaseAddress = new Uri("https://comicvine.gamespot.com/api/");
            client.DefaultRequestHeaders.Add("User-Agent", "HQVerse/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}