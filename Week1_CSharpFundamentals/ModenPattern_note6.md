# C# Modern Pattern Matching: Kodun Sihirli Dönüşümü 

Selam! Arkana yaslan. Bugün seninle C#'ın son yıllarda geçirdiği en büyük, en can yakıcı ama bir o kadar da keyifli evrimlerinden birini konuşacağız: **Modern Pattern Matching (Desen Eşleştirme)**. 

C# 7.0 ile ufak ufak hayatımıza giren bu yapı, özellikle C# 9, 10 ve 11 ile adeta bir zirve yaşadı. Eğer hâlâ eski usul, iç içe geçmiş, insanı hayattan soğutan `if-else` blokları veya `is/as` tip dönüşümleriyle boğuşuyorsan, bu yazıdan sonra koduna bakış açın tamamen değişecek. 

Hazırsan, en temel mantıktan başlayıp kurumsal projelerde hayat kurtaran ileri seviye numaralara doğru akalım!

---

## 1. Zihniyet Değişimi: `switch` Artık Bir Memur Değil, Bir Sanatçı! 🧠

Eski C# dünyasında `switch-case` yapıları birer **deyim (statement)** idi. Yani sadece akışı yönlendirir, içeride bir şeyler yapar ve `break` ile kapıyı kapatıp çıkardı. Kod kalabalığından geçilmezdi.

Modern C# ve .NET dünyasında ise `switch` artık bir **ifadedir (expression)**. Yani doğrudan bir değer üretir ve bunu bir değişkene atayabilir. Bak aradaki farka, ne demek istediğimi çok iyi anlayacaksın:

```csharp
// 🛑 Eski Yöntem (Statement) - Yoran, Kalabalık Yapı
string result;
switch (statusCode)
{
    case 200:
        result = "OK";
        break;
    case 404:
        result = "Not Found";
        break;
    default:
        result = "Unknown";
        break;
}

// ✨ Modern Yöntem (Expression) - Tertemiz, Net!
string result = statusCode switch
{
    200 => "OK",
    404 => "Not Found",
    _   => "Unknown" // İşte buna Discard deseni diyoruz, "her şeyi yakala" demek.
};
```
Gözün gönlün açıldı değil mi? `case` yok, `break` yok, gereksiz parantezler yok. Sadece girdi ve çıktı var.

---

## 2. Gelişmiş Desen Türleri: Koduna Seviye Atlatacak Numaralar 🚀

Mülakatlarda ya da kod incelemelerinde (code review) "İşte bu çocuk işi biliyor" dedirtecek modern desenlere yakından bakalım.

### A) İlişkisel (Relational) ve Mantıksal (Logical) Desenler
C# 9.0 sayesinde artık `switch` içinde bildiğin matematiksel büyüktür/küçüktür (`<`, `>`, `<=`, `>=`) ifadelerini `and`, `or`, `not` gibi çok insani anahtar kelimelerle bağlayabiliyoruz.

**Senaryo:** Bir e-ticaret siten var ve sepetteki ürün sayısına göre kargo ücreti hesaplayacaksın.

```csharp
public decimal CalculateShipping(int itemCount) => itemCount switch
{
    <= 0         => throw new ArgumentException("Hadi ama, ürün sayısı 0 veya eksi olamaz!"),
    > 0 and <= 3 => 50.00m,   // 1, 2 veya 3 ürün varsa
    > 3 and <= 7 => 35.00m,   // 4, 5, 6 veya 7 ürün varsa
    _            => 0.00m     // 7'den fazlaysa kargo bizden! 🎉
};
```

### B) Özellik (Property) Deseni
Bazen sadece gelen nesnenin tipine bakmak yetmez, o nesnenin *içindeki* özelliklerin ne olduğuna da bakmak istersin. İşte burada devreye Property Deseni giriyor.

**Senaryo:** Kullanıcı rollerine ve hesap durumlarına göre panele giriş yetkisi vereceğiz.

```csharp
public record User(string Name, string Role, bool IsActive);

public string GetDashboardAccess(User user) => user switch
{
    { IsActive: false }                => "Hesabınız askıya alınmış, önce onu bir çözelim.",
    { Role: "Admin" }                  => "Kırmızı halılar serildi, yönetim paneline buyurun.",
    { Role: "Editor", IsActive: true } => "İçerik paneli sizi bekler, kaleminiz keskin olsun.",
    { Role: "Customer" }               => "Müşteri paneline yönlendiriliyorsunuz...",
    _                                  => "Hop! Yetkisiz erişim."
};
```
**Perde Arkası:** Sen bu tatlı kodu yazarken, derleyici arka planda `user != null && user.IsActive == false` gibi sıkıcı kontrolleri senin için en optimize şekilde yapıyor. Sen işin keyfine bakıyorsun.

### C) Konumsal (Positional) Desen ve Deconstruction
Eğer sınıflarında veya `record` yapılarında `Deconstruct` metodu tanımlıysa (yani nesne dışarıya tuple olarak parçalanabiliyorsa), nesneyi doğrudan parantez `(x, y)` içindeki sırasına göre eşleştirebilirsin.

**Senaryo:** Bir oyun projesindesin ve 2D koordinat noktasına göre oyuncunun hangi bölgede olduğunu bulman lazım.

```csharp
public record Point(int X, int Y);

public string GetRegion(Point point) => point switch
{
    (0, 0)       => "Tam merkezdesin (Origin)!",
    ( > 0, > 0)  => "1. Bölge (Kuzey Doğu)",
    ( < 0, > 0)  => "2. Bölge (Kuzey Batı)",
    ( _, 0)      => "X Ekseni Üzerinde Bir Yerdersin",
    _            => "Diğer Bölgeler..."
};
```

### D) List Patterns (C# 11.0 ile Gelen Sihir)
Diziler (Arrays) veya `List<T>` gibi koleksiyonların sadece tipine değil; **içeriğine, sırasına ve uzunluğuna** göre eşleştirme yapabilirsin. Özellikle veri işleme (parsing) işlerinde tam bir hayat kurtarıcı.

Burada iki sihirli sembolümüz var:
*   `_` : Tek bir elemanı temsil eder (ne olduğu önemsiz, orada bir şey var işte).
*   `..` : Sıfır veya daha fazla elemanı temsil eden dilim (slice) deseni.

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

bool match = numbers switch
{
    [1, 2, ..]      => true,  // 1 ve 2 ile başlasın, gerisi ne olursa olsun diyenler
    [_, _, 3, _, _] => true,  // Toplam 5 elemanlı olacak ama tam ortadaki eleman 3 olacak diyenler
    [.., 5]         => true,  // Dünyanın sonu 5 ile bitsin yeter ki diyenler
    _               => false
};
```

---

## 3. Gerçek Hayat Senaryosu: Kurumsal Projelerin Ağır Abisi (Kompleks Eşleştirme) 🔥

Şimdiye kadar gördüklerimizi birleştirelim ve gerçek bir kurumsal projede karşımıza çıkabilecek cinsten bir ödeme motoru (Payment Engine) yazalım. Ödeme tipine göre komisyon oranlarını dinamik olarak hesaplayacağız.

```csharp
public abstract record Payment(decimal Amount);
public record CreditCardPayment(decimal Amount, string CardType, bool IsInternational) : Payment(Amount);
public record BankTransferPayment(decimal Amount, string BankCode) : Payment(Amount);
public record CryptoPayment(decimal Amount, string Currency) : Payment(Amount);

public class FeeCalculator
{
    public decimal CalculateFee(Payment payment) => payment switch
    {
        // 1. Kredi kartı ve yurt dışı ise: Sabit %5 + 10 TL komisyon alalım.
        CreditCardPayment { IsInternational: true } cc => (cc.Amount * 0.05m) + 10,
        
        // 2. Kredi kartı, yurt içi ama Premium kart ise: Bizdensin, komisyon yok!
        CreditCardPayment { IsInternational: false, CardType: "Premium" } => 0,
        
        // 3. Standart yurt içi kredi kartı: %2 komisyonunu alırız.
        CreditCardPayment => payment.Amount * 0.02m,
        
        // 4. Banka transferi: Eğer bizim anlaşmalı bankamız (XBANK) ise tamamen ücretsiz.
        BankTransferPayment { BankCode: "XBANK" } => 0,
        
        // 5. Diğer banka transferleri: Sabit 5 TL.
        BankTransferPayment => 5.00m,
        
        // 6. Kripto ödemesi: Bitcoin (BTC) ise geleceğe güvenimizden %1, diğer altcoinler ise %3.
        CryptoPayment { Currency: "BTC" } => payment.Amount * 0.01m,
        CryptoPayment                     => payment.Amount * 0.03m,
        
        // 7. Savunma Sanayii Modu: Null gelirse patlat!
        null => throw new ArgumentNullException(nameof(payment)),
        
        // 8. Güvenlik Duvarı: Sisteme yarın yeni bir ödeme tipi eklenirse gözden kaçmasın.
        _ => throw new NotSupportedException("Henüz bu ödeme yöntemini desteklemiyoruz dostum.")
    };
}
```

### Derleyicinin Gizli Gücü: Exhaustive Matching 🛡️
Yukarıdaki kodda en alttaki o kurtarıcı `_` (default) durumunu yazmadığını hayal et. Derleyici kodunu inceliyor ve diyor ki: *"Kardeşim sen `Payment` türünden her şeyi kabul ediyorsun ama yarın bir gün biri `SodexoPayment` diye bir sınıf eklerse bu switch patlar!"* 

Sana hemen bir uyarı fırlatıyor: 
`warning CS8509: The switch expression does not handle all possible values of its input type.`

İşte modern C#'ın en büyük vizyonu bu. Tip güvenliği (type safety) artık sadece bir kural değil, senin yerine kodun geleceğini düşünen bir asistan.

---

## 🧭 Özet Mantık: Kendine Doğru Soruyu Sor!

Modern Pattern Matching yazarken kafanda kurman gereken tek bir soru var: **"Ben elimdeki bu nesnenin hangi durumuyla ilgileniyorum?"**

*   **Tipiyle mi?** `=> CreditCardPayment cc`
*   **İçindeki bir özellikle mi?** `=> { Property: value }`
*   **Matematiksel aralığıyla mi?** `=> > 10 and < 50`
*   **Koleksiyonun dizilimiyle mi?** `=> [1, 2, ..]`

Hepsini tek bir satırda, bir yapboz gibi birleştirebilirsin. Üstelik "Yahu bu kadar tatlı yazıyoruz ama arkada performansı baltalar mı?" diye de korkma. Derleyici arka planda bunu senin yazabileceğinden çok daha optimize, canavar gibi `if-else` veya `jump table` yapılarına dönüştürür. 

Yani ne performans kaybı var, ne de okunabilirlik sorunu var. Kodun adeta şiir gibi akar! 🚀

Umarım kahven bitmeden bu işi kafanda oturtabilmişizdir. Sence de geçmeye değmez mi? 😉