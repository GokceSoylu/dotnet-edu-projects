# .NET Ekosisteminin Kalbi: LINQ ve Deferred Execution Mantığı

.NET geliştiriciliğinde ileri seviyeye geçişin en kritik eşiklerinden biri, sorguların arkasında dönen mekanizmayı doğru anlamaktır. Bu yazıda LINQ mimarisini, **Deferred Execution (Ertelenmiş Çalışma)** mantığını ve mülakatların vazgeçilmez konusu **IEnumerable / IQueryable** farkını derinlemesine inceliyoruz.

---

### 1. LINQ (Language Integrated Query) Syntax Yapıları

LINQ, koleksiyonlar veya veritabanları üzerinde sorgulama yapmayı C# dilinin birinci sınıf bir öğesi haline getirir. Derleme aşamasında ikisi de aynı kapıya çıksa da iki farklı yazım şekli vardır:

#### A) Query Syntax (Sorgu Sözdizimi)
SQL'e benzer ve deklaratiftir; yani "nasıl yapılacağını" değil, "ne elde etmek istediğinizi" belirtirsiniz. Karmaşık `join` veya `group by` işlemlerinde okunabilirliği yüksektir.

```csharp
List<int> sayilar = new List<int> { 1, 2, 3, 4, 5, 6 };

// Query Syntax
var ciftSayilar = from sayi in sayilar
                  where sayi % 2 == 0
                  select sayi;
```
> **Detay:** SQL'in aksine `from` ile başlar. Bunun sebebi, IDE'nin veri kaynağını önce bilip içerideki property'ler için kod tamamlama (IntelliSense) desteği sunabilmesidir.

#### B) Method Syntax (Metot Sözdizimi / Fluent API)
Extension metotlar ve Lambda Expressions (`=>`) kullanır. Zincirlenebilir (Fluent) yapısı nedeniyle C# geliştiricileri arasında en yaygın kullanılan yöntemdir.

```csharp
// Method Syntax
var ciftSayilarMethod = sayilar.Where(sayi => sayi % 2 == 0);
```

#### Arka Planda Ne Oluyor?
Siz Query Syntax yazsanız bile, C# derleyicisi (Roslyn) bunu derleme zamanında (compile-time) otomatik olarak Method Syntax’a, yani extension metot çağrılarına dönüştürür. Performans olarak aralarında **hiçbir fark yoktur**.

---

