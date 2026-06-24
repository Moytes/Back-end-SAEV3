using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Utilities.Responses;

namespace Utilities.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var isDatabaseUnavailable = IsDatabaseUnavailable(exception);
        context.Response.StatusCode = isDatabaseUnavailable
            ? (int)HttpStatusCode.ServiceUnavailable
            : (int)HttpStatusCode.InternalServerError;

        var response = new ApiResponse<string>(
            context.Response.StatusCode,
            $"ID_{context.Response.StatusCode}",
            isDatabaseUnavailable
                ? "No se pudo conectar con la base de datos. Verifica la conexion a internet, DNS y la cadena SupabaseConnection."
                : exception.Message
        );

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }

    private static bool IsDatabaseUnavailable(Exception exception)
    {
        if (exception is NpgsqlException or DbUpdateException or TimeoutException)
            return true;

        return exception.InnerException != null && IsDatabaseUnavailable(exception.InnerException);
    }
}
