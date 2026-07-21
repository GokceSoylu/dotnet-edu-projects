using System.Net;
using System.Text.Json;
using Week2_MinimalVsController.Exceptions;

namespace Week2_MinimalVsController.Middlewares;

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
            // İstek normal akışında devam eder
            await _next(context);
        }
        catch (Exception ex)
        {
            // Boru hattının herhangi bir yerinde hata çıkarsa burası yakalar
            _logger.LogError(ex, "Uygulamada beklenmeyen bir hata oluştu: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Hata türüne göre HTTP durum kodunu belirliyoruz
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound, // 404
            _ => HttpStatusCode.InternalServerError       // 500
        };

        context.Response.StatusCode = (int)statusCode;

        // Standart Hata Yanıt Formatı
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            Title = statusCode == HttpStatusCode.NotFound ? "Kaynak Bulunamadı" : "Sunucu Hatası",
            Timestamp = DateTime.UtcNow
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}