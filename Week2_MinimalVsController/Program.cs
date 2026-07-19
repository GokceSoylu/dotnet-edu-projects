var builder = WebApplication.CreateBuilder(args);

// 1. Framework'e projedeki Controller sınıflarını taraması ve DI konteynerine eklemesi talimatını veriyoruz
builder.ServiceAppControllers();



// 2. MİNİMAL API ENDPOINT'İ
// Ortada hiçbir sınıf instance'ı türetilmeden, doğrudan HTTP pipeline üzerinde jet hızıyla eşleşir
var app = builder.Build();

// 3. CONTROLLER YÖNLENDİRMESİ
// Gelen isteklerin Controllers klasöründeki sınıflara yönlendirilmesini sağlar
app.MapGet("/api/minimal/hello", () => Ok(new { message = "Hello form minmal API" }));

app.MapControllers();

app.Run();

