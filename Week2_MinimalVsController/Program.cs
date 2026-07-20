using Week2_MinimalVsController.Middlewares;
using Week2_MinimalVsController.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddTransient<ITransientService, LifetimeService>();
builder.Services.AddScoped<IScopedService, LifetimeService>();
builder.Services.AddSingleton<ISingletonService, LifetimeService>();

var app = builder.Build();

// CUSTOM MIDDLEWARE'I BORU HATTINA EKLİYORUZ
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapGet("/api/minimal/hello", () => Results.Ok(new { message = "Hello from Minimal API!" }));

app.MapGet("/api/lifetime", (
    ITransientService transient1,
    ITransientService transient2,
    IScopedService scoped1,
    IScopedService scoped2,
    ISingletonService singleton1,
    ISingletonService singleton2) =>
{
    var result = new
    {
        Transient_Call1 = transient1.GetGuid(),
        Transient_Call2 = transient2.GetGuid(),
        Scoped_Call1 = scoped1.GetGuid(),
        Scoped_Call2 = scoped2.GetGuid(),
        Singleton_Call1 = singleton1.GetGuid(),
        Singleton_Call2 = singleton2.GetGuid()
    };

    return Results.Ok(result);
});

app.MapControllers();

app.Run();