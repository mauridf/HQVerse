using System.Net;
using System.Text.Json;
using HQVerse.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HQVerse.CrossCutting.Exceptions;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case Domain.Exceptions.ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed.";
                errorResponse.Errors = validationEx.Errors;
                _logger.LogWarning(exception, "Validation error: {TraceId}", traceId);
                break;

            case EntityNotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = notFoundEx.Message;
                _logger.LogWarning(exception, "Entity not found: {TraceId}", traceId);
                break;

            case BusinessRuleException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                errorResponse.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                errorResponse.Message = businessEx.Message;
                _logger.LogWarning(exception, "Business rule violation: {TraceId}", traceId);
                break;

            case UnauthorizedAccessException unauthorizedEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access.";
                _logger.LogWarning(exception, "Unauthorized access: {TraceId}", traceId);
                break;

            case InvalidOperationException invalidOpEx:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Message = invalidOpEx.Message;
                _logger.LogWarning(exception, "Conflict: {TraceId}", traceId);
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An unexpected error occurred. Please try again later.";
                _logger.LogError(exception, "Unhandled exception: {TraceId}", traceId);
                break;
        }

        context.Response.ContentType = "application/json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }
}