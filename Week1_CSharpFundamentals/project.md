# Hafta 1: Asenkron Akıllı Sipariş Analizör & Raporlayıcı

Bu proje, .NET 8/9 (C# 12/13) temel özelliklerini, LINQ (IEnumerable/IQueryable) davranışlarını ve asenkron programlama pratiklerini gerçek bir veri tabanı şeması üzerinde pekiştirmek amacıyla tasarlanmıştır.

## 🎯 Hedeflenen Konular ve Uygulama Alanları
1. **Primary Constructors & Records (C# 12/13):** Veri tabanından çekilen sonuçları taşımak için hafif, değişmez (immutable) `record` ve `record struct` yapıları kullanılacaktır. Servis bağımlılıkları sınıflara Primary Constructor ile enjekte edilecektir.
2. **Modern Pattern Matching:** Sipariş durumuna (`status`) ve ödeme yöntemine göre dinamik komisyon, kargo maliyeti ve teslimat durum analizi `switch expressions` kullanılarak yapılacaktır.
3. **IEnumerable vs IQueryable & Deferred Execution:** 
   * `IQueryable` ile PostgreSQL tarafında filtrelenmiş ve optimize edilmiş SQL sorguları çalıştırılacaktır.
   * Veriler belleğe çekildikten sonra `IEnumerable` kullanılarak detaylı raporlama ve gruplama işlemleri tamamlanacaktır.
   * Sorguların ne zaman tetiklendiği (Deferred Execution) konsol logları ile izlenecektir.
4. **Asenkron Programlama (Task/ValueTask & ConfigureAwait):**
   * Veri tabanı ve simülasyon gecikmeleri `async/await` ile asenkron yönetilecektir.
   * Sık çağrılan ve bellek tahsisini (allocation) azaltabileceğimiz yerlerde `ValueTask` tercih edilecektir.
   * CPU-bound veya bağımsız arka plan görevlerinde thread context switching optimizasyonu için `ConfigureAwait(false)` kullanılacaktır.

## 📂 Klasör ve Dosya Yapısı
Mevcut klasör yapımıza sadık kalarak kodlarımızı şu şekilde organize edeceğiz:
* `AppDbContext.cs`: PostgreSQL veri tabanına bağlanacak ve tabloları (`DbSet`) eşleyecek Entity Framework Core bağlamı.
* `Models.cs`: Veri tabanı varlıkları (Entity modelleri) ve veri taşıma nesneleri (`record` yapıları).
* `Services.cs`: Analiz motorunu, veritabanı sorgularını ve raporlama mantığını barındıran asenkron servis katmanı.
* `Program.cs`: Tüm akışı koordine eden, asenkron metotları çağıran ve sonuçları ekrana basan ana giriş noktası.

## 🚀 Yol Haritası (Adım Adım Kodlama)
- [ ] Adım 1: `Models.cs` dosyasının C# 12/13 standartlarında (Record & Primary Constructor) yazılması.
- [ ] Adım 2: `AppDbContext.cs` dosyasının PostgreSQL bağlantısı ile yapılandırılması.
- [ ] Adım 3: `Services.cs` dosyasında iş mantığının, pattern matching kurallarının ve LINQ sorgularının asenkron (ConfigureAwait ile) yazılması.
- [ ] Adım 4: `Program.cs` içinde tüm akışın birleştirilmesi ve test edilmesi.