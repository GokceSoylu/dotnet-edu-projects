# Biz Aslında Ne Yapıyoruz?

Bak dostum, şimdi arkana yaslan ve kahvenden bir yudum al. Bugün .NET dünyasının son yıllarda geçirdiği en büyük zihniyet değişimini konuşuyoruz: "Minimal APIs vs. Controller-Based". İşin jargonu çok ama mesele aslında oldukça insani bir temizlik hikayesi.

---

## 1. Olayın Felsefesi: "Neden Çıktı Bu Minimal API?"

Yıllardır .NET dünyasında bir API yazacağımız zaman refleks olarak gidip `Controllers` diye bir klasör açar, içine `ControllerBase`’den türeyen devasa sınıflar kurardık. Üzerine `[ApiController]`, `[Route]`, `[HttpGet]` gibi bir sürü etiket (attribute) yapıştırırdık. 
*   **Controller Dünyası:** Katı kuralları olan, kurumsal ve ağırbaşlı bir abi. Büyük projelerde düzeni korur ama en ufak bir "Merhaba Dünya" verisi dönmek için bile seni bir ton dosya açmaya zorlar. Arka planda framework, "Acaba bu projede nerelerde controller sınıfı var?" diyerek bütün projeyi (assembly) tarar. Buna **Reflection (Yansıma)** yükü diyoruz.
*   **Minimal API Dünyası:** .NET 6 ile gelen bu yaklaşım ise tam bir "Hafifleyelim" hareketi. Bulut sistemler, mikroservisler ve serverless mimariler popüler olunca, .NET ekibi oturdu düşündü: "Biz neden bir HTTP isteğini çalıştırmak için bu kadar dolambaçlı yollardan geçiyoruz?" dediler. Minimal API, aradaki bütün bürokrasiyi kaldırıyor. Bir HTTP isteği mi geldi? Doğrudan bir lambda fonksiyonuna (yani o an yazdığın koda) bağlıyor. Sınıflar yok, kalıtımlar yok, saf hız var.

---

## 2. Arka Planda Neler Dönüyor? (Boru Hattı & Overhead)

İşte işin mühendislik olarak koptuğu yer burası. Bir tarayıcıdan veya mobilden bizim uygulamaya bir HTTP isteği vurduğunda iki mimari çok farklı yollardan yürür:

### Controller’ın Yolu (Ağır Vasıta)
İstek gelir. Framework yönlendirme tablosuna bakar. "Hah, bu istek şu Controller'a gidecek" der. Sonra gider `Dependency Injection` konteynerını ayağa kaldırır, o controller sınıfından hafızada yeni bir nesne (instance) üretir. Bu sırada bir ton filtreyi tetikler, istekten gelen verileri metoda uydurmak için (Model Binding) yine arka planda gizli bir reflection motoru çalıştırır. Sonunda kodun çalışır ve nesne çöp (Garbage Collector) sırasına girer. Her istekte bu döngü tekrarlanır. İşte bu aradaki fazladan yapılan işlere **Overhead** diyoruz.

### Minimal API’nin Yolu (Spor Araba)
Uygulama ilk ayağa kalktığında, Minimal API senin yazdığın o küçük fonksiyonları derler ve hazır birer delegasyon (Compiled Delegate) olarak belleğe yazar. İstek geldiği an, ortada üretilecek bir sınıf, taranacak bir attribute, geçilecek ağır filtre zincirleri yoktur. İstek doğrudan o hafızadaki fonksiyona çarpar ve jet hızıyla yanıt döner. Hafızada gereksiz nesne üretilmediği için bellek tüketimi (allocation) taban yapar, saniyede karşılanan istek sayısı (throughput) tavan yapar.

---

## 3. Hangisini, Ne Zaman Seçelim?

*   **Küçük/Orta Ölçekli İşler veya Mikroservisler:** Tartışmasız Minimal API. Dosya kalabalığı yapmaz, seni yormaz, performansı uçurur.
*   **Büyük Kurumsal Projeler:** Eğer dikkat etmezsen, Minimal API ile yazılan yüzlerce endpoint `Program.cs` dosyasını tam bir spagetti koda çevirebilir. Tabii ki bunu `.MapGroup()` gibi modüler yapılarla temiz tutmak mümkün ama Controller mimarisinin getirdiği o hazır, katı disiplin bazen devasa ekiplerin işini kolaylaştırabilir.

Kısacası; Minimal API bize "Gereksiz protokollerden kurtul, sadece işini yap" diyor.