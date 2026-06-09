using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HQVerse.CrossCutting.Logging;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString;

        try
        {
            await _next(context);
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMs}ms",
                    method, path, queryString, statusCode, elapsedMs);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMs}ms",
                    method, path, queryString, statusCode, elapsedMs);
            }
        }
        catch (Exception)
        {
            stopwatch.Stop();
            _logger.LogError(
                "HTTP {Method} {Path}{QueryString} failed in {ElapsedMs}ms",
                method, path, queryString, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}