using Week1_CSharpFundamentals; // Modellerimizi buraya çağırıyoruz

Console.WriteLine("=== 1. RECORD (CLASS TABANLI) DENEYİSİ ===");

// İki farklı nesne üretiyoruz ama içindeki değerler tamamen aynı
var product1 = new ProductDto("Telefon", 45000, "Elektronik");
var product2 = new ProductDto("Telefon", 45000, "Elektronik");

// MANTIK 1: Değer Eşitliği (Value Equality)
// Klasik class olsaydı buna False diyecekti. Ama Record olduğu için içerik kıyaslaması yapar:
Console.WriteLine($"product1 ve product2 eşit mi?: {product1 == product2}"); // ÇIKTI: True

// MANTIK 2: Değişmezlik (Immutability)
// product1.Price = 50000; // EĞER BU SATIRI AÇARSAN DERLEME HATASI ALIRSIN! Değiştirilemez.

// MANTIK 3: With İfadesi ile Kopyalama
// Telefonun fiyatı değişti diyelim, eskisini bozmadan yenisini türetiyoruz:
var updatedProduct = product1 with { Price = 48000 };
Console.WriteLine($"Yeni Ürün: {updatedProduct}");
// ÇIKTI otomatik formatlı gelir: ProductDto { Name = Telefon, Price = 48000, Category = Elektronik }


Console.WriteLine("\n=== 2. RECORD STRUCT (VALUE TABANLI) DENEYİSİ ===");

// Belleğin stack bölgesinde yaşayan hafif bir koordinat nesnesi oluşturalım
var konum1 = new GPSCoordinate(41.0082, 28.9784); // İstanbul
var konum2 = new GPSCoordinate(41.0082, 28.9784);

Console.WriteLine($"Konumlar eşit mi?: {konum1 == konum2}"); // ÇIKTI: True

// Bu da readonly olduğu için değiştirilemez, ancak kopyalanabilir:
var konum3 = konum1 with { Latitude = 40.9920 };
Console.WriteLine($"Yeni Konum: {konum3}");