using System.Reflection;
using HQVerse.Application.Interfaces;
using HQVerse.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HQVerse.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

        // Services
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<IComicSeriesService, ComicSeriesService>();
        services.AddScoped<IComicIssueService, ComicIssueService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<ICreatorService, CreatorService>();
        services.AddScoped<IStoryArcService, StoryArcService>();
        services.AddScoped<IUniverseService, UniverseService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IComicVineService, ComicVineSyncService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IScanService, ScanService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IUniverseService, UniverseService>();
        services.AddScoped<ICreatorService, CreatorService>();
        services.AddScoped<IStoryArcService, StoryArcService>();
        services.AddScoped<IFavoriteService, FavoriteService>();

        return services;
    }
}
