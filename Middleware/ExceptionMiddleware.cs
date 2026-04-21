using System.Net;
using System.Text.Json;

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
            await _next(context); // continue request pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = ex.Message
        };

        // 🎯 Map exception → HTTP status code
        context.Response.StatusCode = ex switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,        // 400
            KeyNotFoundException => (int)HttpStatusCode.NotFound,       // 404
            InvalidOperationException => (int)HttpStatusCode.Conflict,  // 409
            _ => (int)HttpStatusCode.InternalServerError               // 500
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}