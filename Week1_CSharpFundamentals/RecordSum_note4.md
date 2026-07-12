# .NET Bellek Yönetimi ve Record Dünyası: Stack, Heap ve Performansın Sihirli Değnekleri

.NET'te ileri seviyeye geçmenin ve yazdığın kodun hakkını vermenin yolu, arka planda RAM'in nasıl çalıştığını bilmekten geçiyor. C#'ta bellek en temel haliyle ikiye ayrılır: **Stack** ve **Heap**. Bu iki bölgenin mimarisini anlamak, `record` ile `record struct` arasındaki o devasa performans farkının da kapısını aralar.

---

## 1. Class'lar ve Record'lar (Heap Bellek)

Bir sınıf (`class`) veya standart bir `record` oluşturduğunda, .NET gider ve RAM'in **Heap** adı verilen büyük, dinamik ama yönetimi daha karmaşık olan bölgesinden bir yer ayırır. 

Heap güzeldir, esnektir ama bir bedeli vardır: İşin bittiğinde o belleğin temizlenmesi gerekir. Bu temizliği .NET dünyasında **Garbage Collector (Çöp Toplayıcı - GC)** yapar. GC, belleği temizlemek için sistemi anlık olarak tarar. Eğer uygulaman saniyede binlerce, milyonlarca `record` nesnesi oluşturup çöpe atıyorsa, Garbage Collector sürekli çalışmak zorunda kalır. Bu da uygulamanın nefesinin kesilmesine, yani **mikro donmalara (GC pauses)** yol açar.

## 2. Struct'lar ve Record Struct'lar (Stack Bellek)

Bir `struct` veya `record struct` oluşturduğunda ise işler tamamen değişir. Bu veri, RAM'in **Stack** adı verilen, donanım seviyesinde çalışan ultra hızlı bölgesinde tutulur. 

Stack'teki verinin ömrü çok nettir: İçinde bulunduğu süslü parantez `{ }` (metot gövdesi, döngü vs.) bittiği an, veri anında yok olur. Garbage Collector'ın ruhu bile duymaz; sisteme hiçbir ek yük binmeden, ışık hızıyla bellekten silinir.

### Gerçek Hayattan Bir Performans Senaryosu
Bir oyun veya harita uygulaması yazdığını, ekranda anlık olarak yer değiştiren **100.000 adet düşmanın ya da aracın X ve Y koordinatlarını** tutman gerektiğini hayal et. Düşmanlar hareket ettikçe de sürekli konumları güncelliyorsun.

*   **Senaryo A - `record` (Class) Kullandın:**
    ```csharp
    public record Position(int X, int Y);
    ```
    Düşmanlar her hareket ettiğinde sürekli yeni konumlar türetiyorsun. RAM'in Heap bölgesinde saniyede milyonlarca `Position` nesnesi doğup ölüyor. Garbage Collector bunları temizlemek için deli gibi çalışıyor, işlemcin tavan yapıyor ve oyunda anlık takılmalar (stuttering) başlıyor.
*   **Senaryo B - `readonly record struct` Kullandın:**
    ```csharp
    public readonly record struct Position(int X, int Y);
    ```
    Yine yeni konumlar türetiyorsun, yine constructor yazmıyorsun, her şey çok konforlu. Ama tek bir farkla: **Bu işlemlerin hepsi Stack bellekte dönüyor.** Sıfır maliyetle bellek temizleniyor ve uygulaman yağ gibi akıyor.

> **Record Struct'ın Esprisi Nedir?**
> Sana `record` yapısının sunduğu o "constructor yazmama, kolay kopyalama, içerik kıyaslama" gibi tüm yazım konforunu sunarken; arka planda bunu hafif, taşınabilir ve ultra performanslı bir **Value Type (Değer Tipi)** olarak Stack üzerinde çalıştırabilmesidir.

---

## 3. Record Dünyasının İki Sihirli Değneği: `==` ve `with`

C# dünyasında `record` yapılarını sıradan sınıflardan ayıran iki harika özellik vardır: `==` operatörü ve `with` ifadesi. Hem `record` (class) hem de `record struct` üzerinde nasıl çalıştıklerine yakından bakalım.

### Sihirli Değnek 1: `==` Operatörü (Değer Tabanlı Eşitlik)
Klasik sınıflarda `==` kullandığında, C# nesnelerin içindeki verilere bakmaz; *"Bu iki nesne RAM'de aynı adresi mi gösteriyor?"* diye bakar (Referans Eşitliği). Record'larda ise derleyici, arka planda tüm property'leri tek tek kıyaslayan özel bir eşitlik mantığı (`Value Equality`) örer.

*   **`record` (Class) Üzerinde:** İki farklı nesne Heap bellekte tamamen farklı iki adreste dursa bile, içindeki veriler aynıysa `==` sonucu `True` döner.
    ```csharp
    // Models.cs
    public record CustomerAccount(string Iban, string OwnerName);

    // Program.cs
    var hesap1 = new CustomerAccount("TR123", "Ahmet");
    var hesap2 = new CustomerAccount("TR123", "Ahmet");

    // Klasik class olsaydı 'False' dönerdi çünkü referanslar farklı.
    // Ama record içerideki verileri tek tek kontrol eder.
    bool classRecordEsitMi = (hesap1 == hesap2); 
    Console.WriteLine($"Hesaplar eşit mi?: {classRecordEsitMi}"); // ÇIKTI: True
    ```
*   **`record struct` Üzerinde:** Geleneksel `struct` yapılarında `==` operatörünü doğrudan kullanamazsın; derleyici hata verir ve gidip elinle `operator overloading` yapman gerekir. `record struct` ise bu ameleliği tamamen bitirir.
    ```csharp
    // Models.cs
    public readonly record struct ColorRgb(int R, int G, int B);

    // Program.cs
    var renk1 = new ColorRgb(255, 0, 0);
    var renk2 = new ColorRgb(255, 0, 0);

    // Normal struct'ta hata verecek olan bu satır doğrudan çalışır:
    bool structRecordEsitMi = (renk1 == renk2);
    Console.WriteLine($"Renkler eşit mi?: {structRecordEsitMi}"); // ÇIKTI: True
    ```

### Sihirli Değnek 2: `with` İfadesi (Non-Destructive Mutation)
Record'ların en büyük mottosu: **"Değiştirme, kopyala ve güncelleyerek yeni bir tane üret!"** Yaklaşımıdır. Buna *yıkıcı olmayan mutasyon* denir. Nesnenin orijinal halini asla bozmazsın, böylece kodun başka yerindeki akışlar bu değişiklikten kötü etkilenmez.

*   **`record` (Class) Üzerinde:** Heap bellekteki mevcut nesnenin tüm verilerini kopyalar, sadece süslü parantez içinde belirttiğin alanları güncelleyerek yeni bir nesne referansı oluşturur.
    ```csharp
    // Models.cs
    public record FlightTicket(string From, string To, string FlightNumber, decimal Price);

    // Program.cs
    var bilet1 = new FlightTicket("IST", "AYT", "TK1234", 1500);
    // Yolcu business class'a geçmek istedi, fiyat değişecek. bilet1'i bozmuyoruz:
    var bilet2 = bilet1 with { Price = 3000 };

    Console.WriteLine($"Orijinal Bilet Fiyatı: {bilet1.Price}"); // ÇIKTI: 1500 (Bozulmadı)
    Console.WriteLine($"Yeni Bilet Fiyatı: {bilet2.Price}");      // ÇIKTI: 3000
    Console.WriteLine($"Yeni Bilet Rotası: {bilet2.From} -> {bilet2.To}"); // ÇIKTI: IST -> AYT (Otomatik kopyalandı)
    ```
*   **`record struct` Üzerinde:** Stack bellekte çalışan yapılar için de `with` ifadesi birebirdir. Özellikle `readonly record struct` kullanarak tamamen değiştirilemez (immutable) yapılar kurduğunda, veriyi güncellemenin tek temiz yolu yine `with` kullanmaktır.
    ```csharp
    // Models.cs
    public readonly record struct GameVector(int X, int Y, int Z);

    // Program.cs
    var oyuncuKonum = new GameVector(10, 20, 0);
    // Oyuncu sadece yukarı zıpladı (Z ekseni değişti), X ve Y aynı kaldı:
    var yeniKonum = oyuncuKonum with { Z = 5 };

    Console.WriteLine($"Eski Konum: {oyuncuKonum}"); // ÇIKTI: GameVector { X = 10, Y = 20, Z = 0 }
    Console.WriteLine($"Yeni Konum: {yeniKonum}");     // ÇIKTI: GameVector { X = 10, Y = 20, Z = 5 }
    ```

---

## 4. Akla Takılan Kritik Sorular

### Soru 1: `with` ile üretilen yeni nesnenin referansı eski nesneyle aynı mıdır?
**Hayır, kesinlikle aynı değildir.** 
`with` ifadesini kullandığın an, .NET arka planda RAM'de (türüne göre Heap veya Stack üzerinde) **tamamen yeni bir yer** ayırır. Eski nesnenin içindeki tüm verileri kopyalar, senin değiştirmek istediğin alanları üzerine yazar ve sana bambaşka bir referans/adres teslim eder.

Bunu C#'ın referans kontrolü yapan `object.ReferenceEquals` metoduyla test edebiliriz:
```csharp
var bilet1 = new FlightTicket("IST", "AYT", "TK1234", 1500);
var bilet2 = bilet1 with { Price = 3000 };

// RAM'deki adreslerini kıyaslayalım:
bool adreslerAyniMi = object.ReferenceEquals(bilet1, bilet2);
Console.WriteLine(adreslerAyniMi); // ÇIKTI: False (Çünkü tamamen farklı nesnelerdir!)
```

### Soru 2: İleride yapılan değişiklikler ikisini de etkiler mi?
**Asla etkilemez.** Çünkü ikisi artık RAM'de tamamen bağımsız iki farklı dünyadır. `bilet2` üzerinde yaptığın hiçbir şey `bilet1`'i etkilemez.

Zaten `record` ve `readonly record struct` yapılarının en büyük amacı bu **Garantili Güvenliktir (Immutability)**. Bir nesneyi oluşturduktan sonra onun alanlarını tek tek doğrudan değiştiremeyeceğin için (`bilet2.Price = 5000;` yazamazsın, derleyici hata verir), ileride kazara bir yerdeki veriyi bozma ihtimalin sıfıra iner.

### Soru 3: Neden `new` yerine `with` kullanıyoruz? Farkı nedir?
En temel fark şudur: `new` sıfırdan, bomboş bir sayfa açarak nesne yaratır. `with` ise var olan bir nesnenin karbon kopya fotoğrafını çekip, sadece istediğin yerleri rötuşlayarak yeni nesne yaratır.

Eğer fiyata zam yapılmış yeni bir bilet üretmek isteseydik ve elimizde sadece `new` olsaydı, eski nesnenin içindeki tüm alanları tek tek elinle yeni nesneye taşımak zorunda kalırdın:
```csharp
var eskiBilet = new FlightTicket("IST", "AYT", "TK1234", 1500);

// new kullanarak kopyalamak tam bir ameleliktir:
var yeniBilet = new FlightTicket(
    eskiBilet.From,          // Elinle kopyaladın
    eskiBilet.To,            // Elinle kopyaladın
    eskiBilet.FlightNumber,  // Elinle kopyaladın
    3000                     // Sadece bunu değiştirdin
);
```
Eğer modelin içinde 20 tane property olsaydı, 19 satırı elinle kopyalamak zorunda kalacaktın. Bu hem zaman kaybı hem de bir alanı unutursan devasa bir hata (bug) kaynağıdır. `with` kullandığında ise .NET o 19 alanı arka planda senin yerine otomatik kopyalar.

---

## 5. C# Dünyasında "Java Bean" Yaklaşımı ve Mimari Değişim

### .NET'te Java Bean tarzı bir yapı var mı?
Evet, var. Hatta .NET dünyasında buna yıllardır **POCO (Plain Old CLR Object)** denir. Tıpkı Java Bean gibi, POCO yapılarında da boş bir constructor bulunur ve property'lerin hem `get` hem `set` blokları açıktır. İstediğin zaman değer atar, istediğin zaman yolda içini değiştirirsin.

```csharp
// .NET'teki "Java Bean" karşılığı: Standart POCO Sınıfı
public class UserPoco
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### Bu durum .NET’in modern felsefesine ters mi?
Tam olarak ters değil ama kullanım yerleri kesin çizgilerle ayrıldı. Aralarında taban tabana zıt bir felsefe var:
*   **Java Bean / POCO Yapısı:** Tamamen **Mutable (Değiştirilebilir)** bir felsefeye sahiptir. Nesneyi yaratırsın ve yolda sürekli içini kurcalayarak değiştirirsin.
*   **Record Yapısı:** Tamamen **Immutable (Değiştirilemez)** felsefeye dayanır. *"Bir kere düzgünce üretilsin, yolda kimse içini çaktırmadan değiştiremesin, değişecekse yeni bir kopya üretilsin"* mantığındadır.

Modern .NET mimarilerinde yaklaşım şu net kurala evrildi: 
> **"Veri tabanına kaydedilecek nesneler (Entity) veri tabanı araçlarının doğası gereği değiştirilebilir (Mutable/POCO) kalabilir; ancak katmanlar arasında akan, API'den gelip giden veri (DTO, Request, Response) kesinlikle değiştirilemez (Immutable/Record) olmalıdır."**

Böylece kodun katmanları arasında gezen verinin yolda kazara mutasyona uğramasını engeller, çok daha güvenli ve temiz bir sistem inşa etmiş olursun.