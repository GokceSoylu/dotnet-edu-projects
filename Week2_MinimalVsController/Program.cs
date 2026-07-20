using Week2_MinimalVsController.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// SERVİS ÖMÜRLERİNİ CONTAINER'A KAYDEDİYORUZ
builder.Services.AddTransient<ITransientService, LifetimeService>();
builder.Services.AddScoped<IScopedService, LifetimeService>();
builder.Services.AddSingleton<ISingletonService, LifetimeService>();

var app = builder.Build();

app.MapGet("/api/minimal/hello", () => Results.Ok(new { message = "Hello from Minimal API!" }));

// DI TEST ENDPOINT'I
// Aynı isteğin içinde her servisten İKİŞER DEFA çağırıyoruz ki Scoped ve Transient farkı net görülsün
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
        Transient_Call2 = transient2.GetGuid(), // Farklı çıkacak (Her çağrıda yeni)

        Scoped_Call1 = scoped1.GetGuid(),
        Scoped_Call2 = scoped2.GetGuid(),       // Aynı çıkacak (Aynı istek içinde sabit)

        Singleton_Call1 = singleton1.GetGuid(),
        Singleton_Call2 = singleton2.GetGuid()   // Aynı çıkacak (Uygulama boyunca sabit)
    };

    return Results.Ok(result);
});

app.MapControllers();

app.Run();