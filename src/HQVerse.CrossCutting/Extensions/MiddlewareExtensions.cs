using HQVerse.CrossCutting.Exceptions;
using HQVerse.CrossCutting.Logging;
using Microsoft.AspNetCore.Builder;

namespace HQVerse.CrossCutting.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }

    public static IApplicationBuilder UseAllCustomMiddlewares(this IApplicationBuilder app)
    {
        return app
            .UseCorrelationId()
            .UseRequestLogging()
            .UseGlobalExceptionHandler();
    }
}