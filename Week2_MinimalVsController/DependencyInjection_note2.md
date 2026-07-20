## 4. .NET'in Kalbi: Dependency Injection ve Servis Ömürleri

Bak dostum, projedeki servisleri (veri tabanına giden, log atan veya iş mantığı yürüten sınıfları) yönetmek için framework'e deriz ki: "Bu sınıfın nesnesini (instance) ben `new()` anahtar kelimesiyle elle üretmek istemiyorum. Sen bunu RAM'de benim yerime yönet, lazım olana enjekte et." 

İşte framework bu nesneleri RAM'de yönetirken bize 3 farklı yaşam döngüsü seçeneği sunar:

1. **Transient (Uçarı/Günübirlikçi):** `builder.Services.AddTransient<...>()`
   * **Teknik Olayı:** En hafif ve en az sorumluluk alan döngüdür. Bu servise ne zaman, nerede ihtiyaç duyulursa (ister aynı HTTP isteğinin içinde olsun, ister yeni bir istekte) framework GİDER VE BAŞTAN YENİ BİR NESNE ÜRETİR. İş bitince nesne çöpe gider.
   * **Kahve Üslubu:** Her kahve siparişinde sıfırdan yeni bir karton bardak çıkartıp kullanmak gibidir. İş bitince çöpe atılır, bir sonraki sefere asla eskisi kullanılmaz.

2. **Scoped (İstek Başına):** `builder.Services.AddScoped<...>()`
   * **Teknik Olayı:** Gelen tek bir HTTP isteği (Request) boyunca hafızada sadece BİR TEK nesne üretilir. O HTTP isteği sunucu içinde dönüp dururken 10 farklı yerde bu servis çağrılsa bile hep aynı nesne kullanılır. Ama istek bitip istemciye yanıt (Response) döndüğü an o nesne ölür. Yeni bir HTTP isteği gelirse, yeni bir nesne üretilir.
   * **Kahve Üslubu:** Kafeye gelip oturdun, sana bir adisyon açtılar. Masada oturduğun sürece (istek boyunca) her kahve isteyişinde o aynı adisyona yazılır. Sen kafeden çıkıp gittiğinde (istek bittiğinde) o adisyon kapatılır ve çöpe gider. Yeni müşteri gelirse yeni adisyon açılır.

3. **Singleton (Ölümsüz/Tek Tabanca):** `builder.Services.AddSingleton<...>()`
   * **Teknik Olayı:** Uygulama ilk `dotnet run` ile ayağa kalktığı andan, uygulamayı `Ctrl+C` ile kapatana kadar hafızada SADECE BİR KEZ nesne üretilir. Gelen milyonlarca farklı HTTP isteği, binlerce farklı kullanıcı hep o RAM'deki tek bir nesneyi ortaklaşa kullanır.
   * **Kahve Üslubu:** Kafenin girişindeki o devasa espresso makinesi gibidir. Kafe açılırken bir kere kurulur; gelen her müşteri, her sipariş aynı makineyi ortaklaşa kullanır. Kafe kapanana kadar o makine değişmez.

### Ciddi Tehlike: Captive Dependency (Esir Bağımlılık)
Eğer sen gidip hafızada ölümsüz olan bir `Singleton` servisin içerisine, ömrü kısa olan ve istek başına yaşayan bir `Scoped` nesneyi enjekte etmeye çalışırsan, .NET sana kızar ve hata fırlatır. Çünkü Singleton ölemediği için, içindeki Scoped nesneyi de RAM'de esir tutar ve onun da ölmesine izin vermez. Bu da bellek sızıntılarına (Memory Leak) yol açar.