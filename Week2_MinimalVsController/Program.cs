using Microsoft.AspNetCore.Http.HttpResults;
using Week2_MinimalVsController.DTOs;
using Week2_MinimalVsController.Exceptions;
using Week2_MinimalVsController.Middlewares;
using Week2_MinimalVsController.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddTransient<ITransientService, LifetimeService>();
builder.Services.AddScoped<IScopedService, LifetimeService>();
builder.Services.AddSingleton<ISingletonService, LifetimeService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// ÖRNEK VERİ LİSTESİ (Dummy Data)
var products = new List<ProductDto>
{
    new(1, "Laptop", 25000),
    new(2, "Kablosuz Klavye", 1200),
    new(3, "Oyuncu Faresi", 850)
};

// --------------------------------------------------------------------------
// PERŞEMBE PRATİĞİ: TypedResults ve DTO/ApiResponse Kullanımı
// --------------------------------------------------------------------------

// 1. Tüm Ürünleri Getir (TypedResults.Ok)
app.MapGet("/api/products", () =>
{
    var response = ApiResponse<List<ProductDto>>.Success(products, "Ürün listesi getirildi.");
    return TypedResults.Ok(response);
});

// 2. Id'ye Göre Ürün Getir (TypedResults ve Result Pattern)
app.MapGet("/api/products/{id:int}", Results<Ok<ApiResponse<ProductDto>>, NotFound<ApiResponse<ProductDto>>> (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null)
    {
        var failResponse = ApiResponse<ProductDto>.Fail($"ID değeri {id} olan ürün bulunamadı.");
        return TypedResults.NotFound(failResponse);
    }

    var successResponse = ApiResponse<ProductDto>.Success(product, "Ürün detayı getirildi.");
    return TypedResults.Ok(successResponse);
});

// 3. Yeni Ürün Ekle (Created Response)
app.MapPost("/api/products", (ProductDto newProduct) =>
{
    products.Add(newProduct);
    var response = ApiResponse<ProductDto>.Success(newProduct, "Yeni ürün başarıyla eklendi.");
    return TypedResults.Created($"/api/products/{newProduct.Id}", response);
});

app.MapControllers();

app.Run();