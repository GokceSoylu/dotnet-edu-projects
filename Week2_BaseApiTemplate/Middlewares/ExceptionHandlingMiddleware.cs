using System.Net;
using System.Text.Json;
using Week2_BaseApiTemplate.DTOs;
using Week2_BaseApiTemplate.Exceptions;

namespace Week2_BaseApiTemplate.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Uygulamada bir hata oluştu: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            InsufficientBalanceException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Sunucu tarafında beklenmeyen bir hata oluştu.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<string>.Fail(message);
        var jsonResponse = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(jsonResponse);
    }
}