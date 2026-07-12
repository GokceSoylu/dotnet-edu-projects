# C# 12+: Primary Constructors ☕🚀

selam! Kahveni aldıysan, bugün C# dünyasında son zamanlarda yazım konforumuzu inanılmaz artıran, kodun o hamallık kısmını (boilerplate dediğimiz o sıkıcı satırları) çöpe attıran harika bir konuyu konuşacağız: **Primary Constructors (Birincil Yapıcı Metotlar)**. 

Bunu teorik bir doküman gibi değil, mülakatlarda karşına çıktığında masaya vurup "İşin aslı şudur" diyebileceğin, günlük projelerinde ise "Gereksiz kod yazmaktan beni nasıl kurtarır?" sorusuna yanıt bulacağın pratik bir rehber gibi düşünebilirsin.

---

### 1. Nedir Bu Primary Constructors? 🤔
C# 12 ile hayatımıza giren bu özellik, aslında tamamen "yazılımcı dostu" bir sadeleştirme operasyonu. Eskiden bir sınıfa bağımlılık eklemek ya da dışarıdan bir veri almak istediğimizde, sınıfın tepesinde `private readonly` alanlar tanımlar, sonra aşağıda constructor açar ve gelen parametreleri bu alanlara tek tek atardık. 

Primary Constructors sayesinde artık parametreleri doğrudan **sınıfın (class) veya yapının (struct) isminin yanına** yazıp geçiyoruz. Arka plandaki o sıkıcı eşleme işini tamamen derleyiciye (compiler) devrediyoruz.

---

### 2. Peki Neden Bu Yapıyı Seçmeliyiz? (Pratik Case'ler) 🎯

#### 📉 Case A: "Boilerplate" Kod Hamallığından Kurtulmak
Küçük bir projede bile onlarca servis yazıyoruz. Her serviste aynı atama işlemlerini yapmak hem vakit alıyor hem de kod kalabalığı yaratıyor. 15 satırlık bir constructor kodunu tek bir satıra indirmek sence de harika değil mi?

#### 🔌 Case B: Dependency Injection (DI) Kolaylığı
Özellikle Clean Architecture veya katmanlı mimarilerde en çok yaptığımız şey servisleri enjekte etmektir (`ILogger`, `DbContext`, `IEmailService` vb.). Bu yapı, mimari bağımlılıkları sınıfa enjekte ederken tertemiz, okuması ve yönetmesi çok kolay bir yazım sunuyor.

---

### 3. Eski Usul vs Yeni Usul (Kod Karşılaştırması) 💻

Gel ne demek istediğimi kod üzerinde görelim. Diyelim ki bir sipariş servisimiz var ve içeride loglama yapmak istiyoruz:

#### ❌ Eski Usul (Geleneksel Yöntem):
```csharp
public class OrderServiceOld
{
    private readonly ILogger _logger;

    // Sırf şu eşlemeyi yapmak için yazılan satırlara bak...
    public OrderServiceOld(ILogger logger)
    {
        _logger = logger;
    }

    public void ProcessOrder()
    {
        _logger.LogInformation("Sipariş işleniyor (Eski yöntem)...");
    }
}
```

#### Enjeksiyonu Yap, Gerisini Unut! ✨
```csharp
public class OrderService(ILogger logger)
{
    // 'logger' parametresini sınıf içindeki tüm metotlarda doğrudan, ekstra hiçbir şey tanımlamadan kullanabilirsin!
    public void ProcessOrder()
    {
        logger.LogInformation("Sipariş tek satırla tertemiz işlendi!");
    }
}
```

---

### 4. Mülakatların Elit Sorusu: Record ve Class Arasındaki Kritik Fark! ⚠️

Bak burası çok önemli, mülakatlarda kıdemli yazılımcılar tam buradan vurmayı sever. Dışarıdan bakınca `record` ile `class` içindeki kullanımı tamamen aynı görünür ama derleyici arka planda bambaşka dünyalar yaratır:

#### 🟢 `record Product(string Name);`
Derleyici bunu gördüğü an, bu parametreden dışarıya açık, okunabilir ve sadece ilk değer ataması yapılabilir bir **Property** üretir: `public string Name { get; init; }`. 
* **Sonuç:** Nesneye dışarıdan `nesne.Name` diyerek rahatça erişebilirsin.

#### 🔵 `class ProductService(string connectionString);`
Derleyici burada dışarıya açık bir property **üretmez**. Bu parametre, sınıfın içinde gizli, değiştirilemez (`private readonly` benzeri) bir alan (field) gibi davranır.
* **Sonuç:** Dışarıdan `servis.connectionString` yazamazsın, hata alırsın! Bu değişkene sadece sınıfın kendi metotları içinden erişebilirsin.

---

### 🔥 Kahve Arası Altın Kural (Özet)

Eğer amacın sadece veri taşımaksa (DTO, Request/Response nesneleri gibi) ve verilere dışarıdan erişilsin istiyorsan **`record`** ile; eğer bir iş mantığı (business logic) servisi yazıyorsan, bağımlılıkları yönetip dış dünyaya bu servis değişkenlerini kapatmak istiyorsan **`class`** ile Primary Constructor kullanmalısın.

Kodun sade, kahven taze olsun! Bir sonraki teknik kahve sohbetinde görüşmek üzere. ☕🚀