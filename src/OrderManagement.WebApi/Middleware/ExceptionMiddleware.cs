using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace OrderManagement.WebApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string message;
        object details;

        switch (exception)  
        {
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Validation failed";
                details = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Unauthorized";
                details = "Доступ запрещён";
                break;

            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                message = "Not Found";
                details = "Ресурс не найден";
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "Internal Server Error";
                details = "Произошла ошибка на сервере";
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = new
        {
            statusCode,
            message,
            details
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}