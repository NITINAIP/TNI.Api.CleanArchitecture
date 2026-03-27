using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TNI.Api.CleanArchitecture.Application.Exceptions;
using ValidationException = TNI.Api.CleanArchitecture.Application.Exceptions.ValidationException;

namespace TNI.Api.CleanArchitecture.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve => (StatusCodes.Status422UnprocessableEntity, "Validation Failure", ve.Errors),
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found", (IDictionary<string, string[]>?)null),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict", (IDictionary<string, string[]>?)null),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", (IDictionary<string, string[]>?)null),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Forbidden", (IDictionary<string, string[]>?)null),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", (IDictionary<string, string[]>?)null)
        };

        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = _env.IsDevelopment() ? exception.Message : null,
            Instance = context.Request.Path
        };

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
