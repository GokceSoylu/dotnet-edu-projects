using Microsoft.Extensions.Options;
using Week2_MinimalVsController.Configurations;
using Week2_MinimalVsController.DTOs;
using Week2_MinimalVsController.Exceptions;
using Week2_MinimalVsController.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// --------------------------------------------------------------------------
// OPTIONS PATTERN KAYDI
// appsettings.json içindeki "EmailSettings" bölümünü C# sınıfı ile eşliyoruz
// --------------------------------------------------------------------------
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// --------------------------------------------------------------------------
// OPTIONS PATTERN TEST ENDPOINT'İ
// --------------------------------------------------------------------------
app.MapGet("/api/config/email", (IOptions<EmailSettings> emailOptions) =>
{
    // Value özelliği üzerinden güçlü tipli (strongly-typed) nesnemize erişiyoruz
    var settings = emailOptions.Value;

    var response = ApiResponse<EmailSettings>.Success(settings, "E-posta ayarları appsettings.json üzerinden başarıyla okundu.");
    return TypedResults.Ok(response);
});

app.MapControllers();

app.Run();