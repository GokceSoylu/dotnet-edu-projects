# Hafta 2: Minimal APIs vs. Controller-Based Mimarisi

Bu çalışmada, .NET dünyasındaki iki temel HTTP endpoint yönetim yaklaşımının teorik altyapısı, mimari felsefesi ve performans karakteristikleri incelenmiştir.

---

## 1. Mimari Felsefe ve Ortaya Çıkış Amaçları

### Controller-Based API Mimari Felsefesi
* **Geleneksel ve Yapılandırılmış:** ASP.NET Core MVC kökenli, katı kuralları olan ve nesne yönelimli (OOP) prensiplere sıkı sıkıya bağlı bir yapıdır.
* **Büyük Kurumsal Sistemler:** Kodun belirli kalıplarla (Controllers, Actions, Filters) ayrıştırılması, devasa ekiplerin çalıştığı monolitik projelerde düzen sağlar.
* **Yansıma (Reflection) Bağımlılığı:** Framework, çalışma zamanında (runtime) hangi sınıfların controller olduğunu anlamak için assembly'leri taramak zorundadır.

### Minimal API Mimari Felsefesi (.NET 6+)
* **Performans ve Hafiflik:** Bulut bilişim (Cloud-Native), mikroservisler ve serverless mimarilerin yükselişiyle birlikte gelen "gereksiz yüklerden arınma" ihtiyacından doğmuştur.
* **Fonksiyonel Yaklaşım:** Karmaşık sınıf hiyerarşileri kurmak yerine, bir HTTP isteğini doğrudan bir lambda fonksiyonuna veya metoda eşler.
* **Performans Odaklılık:** .NET ekibinin en az bellek tahsisi (low allocation) ve en yüksek throughput (saniye başına istek) hedeflerinin bir sonucudur.

---

## 2. Boru Hattı (Pipeline) ve Overhead (Ek Yük) Farkları

Bir HTTP isteği uygulamaya ulaştığında iki mimarinin arka planda işlettiği süreçler performans farkını doğrudan belirler:

### Controller-Based Pipeline (Ağır Yük)
1. **İstek Karşılama:** Kestrel sunucusu isteği alır.
2. **Endpoint Yönlendirme:** Yönlendirme tablosundan ilgili controller bulunur.
3. **Controller Aktivasyonu (Overhead):** İlgili controller sınıfından `Dependency Injection` konteyneri kullanılarak yeni bir instance (örnek) türetilir. **(Bellek tahsisi ve GC yükü oluşturur)**.
4. **Action Selection:** Sınıf içindeki hangi metodun (Action) çalışacağı belirlenir.
5. **Filtre Zinciri (Action/Result Filters):** Varsa ek filtreler tetiklenir.
6. **Model Binding:** İstek parametreleri reflection ile metodun parametrelerine dönüştürülür.
7. **Çalıştırma ve Yanıt:** Metot çalıştırılır, `IActionResult` nesnesi işlenir.

### Minimal API Pipeline (Hafif Yük)
1. **İstek Karşılama:** Kestrel sunucusu isteği alır.
2. **Doğrudan Yönlendirme (Compiled Delegate):** Uygulama başlarken derlenmiş ve optimize edilmiş olan delegasyon doğrudan çağrılır.
3. **Bypass Edilen Süreçler:** Ortada instantiate edilecek (türetilecek) bir controller sınıfı yoktur. Reflection yükü, action tarama ve ağır filtre zincirleri tamamen bypass edilir.
4. **Doğrudan Çalıştırma:** İstek parametreleri kaynak kod üreteçleri (Source Generators) yardımıyla çok daha hızlı bağlanır ve yanıt doğrudan iletilir.

---

## 3. Kod Okunabilirliği ve Bakım (Maintainability) Dengesi

* **Küçük ve Orta Ölçekli Yapılar:** Minimal API, tüm yönlendirmeleri tek bir yerde veya modüler genişletme metotlarında (`Extension Methods`) görmeyi sağlayarak kod okunabilirliğini artırır. Dosya enflasyonunu (her endpoint için yeni sınıflar açma zorunluluğunu) engeller.
* **Büyük Ölçekli Yapılar:** Eğer doğru mimari kurgulanmazsa, yüzlerce endpoint'in Minimal API ile yönetilmesi `Program.cs` dosyasının bir "kod çöplüğüne" dönüşmesine yol açabilir. Bu durumu engellemek için `.MapGroup()` yapısı ve temiz kod (Clean Code) prensipleriyle uç noktaları modüllere bölmek hayati önem taşır.