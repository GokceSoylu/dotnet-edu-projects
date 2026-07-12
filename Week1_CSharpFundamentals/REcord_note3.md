# C# 9 ve 10: Record ve Record Struct Yapıları

C# 9 ve 10 ile gelen bu yapılar, "sadece veri taşımak" amacıyla oluşturulan nesneler için yazmamız gereken amelelik kodlarını (boilerplate) ortadan kaldırır. Aradaki fark, tamamen Stack-Heap ve Değer-Referans kurallarına dayanır.

---

## 1. record (Aslında bir Sınıftır / Referans Tipidir)

Düz `record` yazdığınızda, C# arka planda bunu bir `class` (referans tipi) olarak derler. Yani veriler **Heap** bölgesinde tutulur.

Ancak normal sınıflardan çok büyük bir farkı vardır: **Değer tabanlı eşitlik (Value Equality)**. Normalde iki farklı sınıf nesnesinin içindeki veriler aynı bile olsa, hafızadaki adresleri farklı olduğu için `==` ile karşılaştırdığınızda `false` döner. `record` ise içindeki verilere bakar.

```csharp
// Tek satırda referans tipli bir record tanımladık
public record Ogrenci(string Isim, int Yas);

Ogrenci ogr1 = new Ogrenci("Ahmet", 20);
Ogrenci ogr2 = new Ogrenci("Ahmet", 20);

// Normal class olsaydı 'False' dönerdi çünkü adresleri farklı.
// Ama record olduğu için içindeki verilere bakar ve 'True' döner!
Console.WriteLine(ogr1 == ogr2); // Çıktı: True
```

Ayrıca default olarak **immutable** (değiştirilemez) gelirler. Yani `ogr1.Yas = 21;` yapamazsınız, veriyi korur.

---

## 2. record struct (Bir Yapıdır / Değer Tipidir)

C# tasarımcıları baktılar ki record özelliğinin bu değer tabanlı karşılaştırması ve pratik yazımı çok sevildi, *"Bunu değer tipleri için de yapalım"* dediler. İşte `record struct` tam olarak budur.

Arka planda bir `struct` (değer tipi) olarak derlenir. Yani veriler doğrudan **Stack** üzerinde yaşar.

```csharp
// Değer tipli bir record struct tanımladık
public record struct Nokta(int X, int Y);

Nokta n1 = new Nokta(5, 10);
Nokta n2 = n1; // Değer tipi olduğu için veri tamamen KOPYALANDI (Stack'te iki ayrı veri var)

n2.X = 50; // n2'yi değiştirmek n1'i ETKİLEMEZ!

Console.WriteLine(n1.X); // Çıktı: 5
Console.WriteLine(n2.X); // Çıktı: 50
```

> 💡 **Küçük bir detay:** `record struct`lar düz `record`ların aksine default olarak değiştirilebilirdir (mutable). Eğer değiştirilemez yapmak isterseniz `readonly record struct` yazmanız gerekir.

---

## Özetle Hangisini, Nerede Seçmelisiniz?

| Özellik | record (veya record class) | record struct |
| :--- | :--- | :--- |
| **Tipi** | Referans Tipi (Heap) | Değer Tipi (Stack) |
| **Kopyalama Davranışı** | Sadece adresi kopyalar. | Tüm veriyi sıfırdan kopyalar. |
| **En İyi Kullanım Yeri** | Veritabanından çekilen büyük veriler, DTO'lar, karmaşık nesneler. | Koordinatlar (X,Y), RGB renk kodları, çok sık oluşturulup atılan küçük veri paketleri. |
| **Değiştirilebilirlik** | Default olarak değiştirilemez (init-only). | Default olarak değiştirilebilir. |

Eğer hafıza yönetimi (Stack/Heap) açısından performans kritik bir uygulamanız yoksa, C# dünyasında genellikle veri transferleri için **düz record** (referans tipi olan) tercih edilir.