###  Deferred Execution (Ertelenmiş Çalışma) Mantığı

Bir LINQ sorgusu oluşturduğunuzda sorgu **hemen çalışmaz**. İlgili satır tetiklendiğinde sadece bir sorgu planı oluşturulur. Gerçek veri filtreleme işlemi, siz o sorgunun sonuçlarını tüketmeye (iterasyona) zorladığınız an gerçekleşir.

```csharp
var rakamlar = new List<int> { 1, 2, 3 };

// Sorgu TANIMLANIYOR (Henüz liste taranmadı)
var sorgu = rakamlar.Where(x => x > 1); 

// Orijinal listeye yeni eleman ekliyoruz
rakamlar.Add(4);

// Sorgu burada ÇALIŞTIRILIYOR (Foreach sorguyu tetikler)
foreach (var rakam in sorgu)
{
    Console.WriteLine(rakam); 
}
// Çıktı: 2, 3, 4
```

Sorgu tanımlandığı an çalışsaydı çıktı sadece `2, 3` olacaktı. Ertelenmiş çalışma sayesinde sorgu, `foreach` satırına gelindiğinde listenin **güncel haline** bakar. Bu durum bellek (memory) ve performans yönetiminde büyük avantaj sağlar.

#### Mekanizma Nasıl İşler? `IEnumerable` ve `yield`
LINQ metotlarının (Where, Select vb.) geri dönüş tipi genellikle `IEnumerable<T>`'dir. Arka planda `yield return` mekanizması çalışır. `foreach` ile dönerken tüm liste belleğe kopyalanmaz; elemanlar veri kaynağından teker teker işlenerek bir akış (stream) şeklinde getirilir.

---

###  Immediate Execution (Hemen Çalıştırma)

Sorgunun ertelenmesini istemiyor ve o anki verinin anlık durumunu (snapshot) almak istiyorsanız sorguyu hemen çalıştırmalısınız. Bunu sağlayan başlıca metotlar:

*   **Koleksiyona dönüştürenler:** `.ToList()`, `.ToArray()`, `.ToDictionary()`
*   **Tekil değer döndürenler:** `.Count()`, `.First()`, `.Sum()`, `.Any()`

```csharp
var rakamlar = new List<int> { 1, 2, 3 };

// .ToList() sorguyu hemen çalıştırır ve sonucu belleğe yazar
var immediateSorgu = rakamlar.Where(x => x > 1).ToList(); 

rakamlar.Add(4);

foreach (var rakam in immediateSorgu)
{
    Console.WriteLine(rakam); 
}
// Çıktı: 2, 3 (4 dahil edilmedi, çünkü sorgu Add işleminden önce tamamlandı)
```

---

###  Mimari Tuzak: IEnumerable vs IQueryable Farkı

Ertelenmiş çalışma iki yapıda da vardır ancak verinin işlenme yeri tamamen farklıdır. Mülakatlarda ve performans optimizasyonlarında en çok bu ayrıma dikkat edilir.

#### IEnumerable (Bellek İçi - In-Memory Sorgular)
*   Veriyi dış kaynaktan (örneğin veritabanından) ham olarak belleğe (RAM) çeker.
*   Filtreleme işlemi **uygulamanın çalıştığı sunucunun belleğinde** yapılır.
*   LINQ metotları `Func<T, bool>` (derlenmiş C# kod blokları) kabul eder.

#### IQueryable (Veritabanı - Out-of-Memory Sorgular)
*   Entity Framework Core gibi ORM yapılarıyla veritabanı sorgularında kullanılır.
*   LINQ metotları `Expression<Func<T, bool>>` yani bir **İfade Ağacı (Expression Tree)** kabul eder.
*   Siz `.Where()` yazdığınızda C# kodu çalışmaz. Yazdığınız ifade bir ağaç yapısına dönüştürülür ve sorgu tetiklendiği an (`ToList()` dendiğinde) bu ağaç EF Core tarafından hedef veritabanının diline (SQL) çevrilir. Filtreleme **veritabanı sunucusunda** yapılır, uygulamaya sadece filtrelenmiş sonuç gelir.

#### Performans Katili Hata Örneği:

```csharp
// ❌ YANLIŞ YAKLAŞIM
IEnumerable<User> users = dbContext.Users.AsEnumerable(); 
var activeUsers = users.Where(u => u.IsActive).ToList(); 
// Açıklama: AsEnumerable() yüzünden milyonlarca satırlık tüm Users tablosu 
// önce SQL'den RAM'e çekilir, sonra filtreleme RAM'de yapılır. Sunucu çöpe döner.

//  DOĞRU YAKLAŞIM
IQueryable<User> users = dbContext.Users.AsQueryable();
var activeUsers = users.Where(u => u.IsActive).ToList();
// Açıklama: SQL'e doğrudan "SELECT * FROM Users WHERE IsActive = 1" cümlesi gider.
// Sunucuya sadece ihtiyacımız olan filtrelenmiş veri gelir.