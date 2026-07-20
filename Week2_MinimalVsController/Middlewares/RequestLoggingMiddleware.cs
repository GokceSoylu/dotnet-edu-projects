using System.Diagnostics;

namespace Week2_MinimalVsController.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    // _next: Boru hattındaki bir sonraki middleware'i temsil eder
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. İstek girdiği an kronometreyi başlatıyoruz
        var stopwatch = Stopwatch.StartNew();

        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;

        _logger.LogInformation("--> [GELEN İSTEK] {Method} {Path}", requestMethod, requestPath);

        // 2. İsteyi boru hattındaki bir sonraki middleware'e (ve nihayetinde Controller/Endpoint'e) devrediyoruz
        await _next(context);

        // 3. İstek işlenip yanıt geri dönerken kronometreyi durduruyoruz
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("<-- [YANIT TAMAMLANDI] {Method} {Path} | Durum Kodu: {StatusCode} | Süre: {Elapsed} ms",
            requestMethod, requestPath, statusCode, elapsedMs);
    }
}