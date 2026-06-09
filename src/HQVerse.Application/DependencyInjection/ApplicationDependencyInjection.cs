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
        services.AddScoped<IComicIssueService, ComicIssueService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}