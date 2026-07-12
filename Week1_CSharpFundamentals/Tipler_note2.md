# C#'ta Değer ve Referans Tipleri (Value & Reference Types)

C#'ta değişkenler, veriyi hafızada (RAM) nasıl sakladıklarına ve yönettiklerine göre iki ana gruba ayrılır: **Değer Tipleri (Value Types)** ve **Referans Tipleri (Reference Types)**. Aralarındaki farkı anlamak, programınızın performansını optimize etmek ve kodun çalışma mantığını doğru çözmek için kritik bir öneme sahiptir.

---

## 1. Değer Tipleri (Value Types)

Değer tipleri, verinin **kendisini** doğrudan hafızanın **Stack (Yığın)** adı verilen hızlı bölgesinde saklar. Bir değer tipini başka bir değişkene atadığınızda (kopyaladığınızda), verinin tamamen yeni ve bağımsız bir kopyası oluşturulur. Dolayısıyla, bir değişken üzerinde yaptığınız değişiklik diğer değişkeni kesinlikle etkilemez.

* **Hafıza Konumu:** Stack (Yığın)
* **Sık Kullanılan Tipler:** `int`, `float`, `double`, `bool`, `char`, `struct` (yapılar) ve `enum`.

### Örnek Senaryo

```csharp
int a = 10;
int b = a; // 'a'nın içindeki 10 değeri 'b'ye kopyalandı.

b = 20;    // 'b' değerini değiştirdik.

Console.WriteLine(a); // Çıktı: 10 ('a' bu değişiklikten etkilenmedi)
Console.WriteLine(b); // Çıktı: 20
```

---

## 2. Referans Tipleri (Reference Types)

Referans tipleri, verinin asıl kendisini **Heap (Öbek)** adı verilen dinamik hafıza bölgesinde saklar. **Stack** bölgesinde ise o verinin Heap'teki adresini (yani referansını/işaretçisini) tutar.

Bir referans tipini başka bir değişkene kopyaladığınızda, verinin kendisi kopyalanmaz; sadece Heap'teki **adres bilgisi** kopyalanır. Bu yüzden iki değişken de hafızadaki aynı veriyi (nesneyi) işaret etmeye başlar. Birinde yaptığınız değişiklik otomatik olarak diğerine de yansır.

* **Hafıza Konumu:** Verinin aslı **Heap**'te, adresi (referansı) ise **Stack**'te tutulur.
* **Sık Kullanılan Tipler:** `class` (sınıflar), `string`, `array` (diziler), `interface` ve `delegate`.

### Örnek Senaryo

```csharp
// Varsayalım ki 'Araba' adında bir sınıfımız (class) var.
Araba araba1 = new Araba();
araba1.Renk = "Kırmızı";

Araba araba2 = araba1; // Nesne kopyalanmadı, sadece adresi kopyalandı! İkisi de aynı arabayı gösteriyor.

araba2.Renk = "Mavi"; // araba2 üzerinden rengi değiştirdik.

Console.WriteLine(araba1.Renk); // Çıktı: Mavi (Çünkü ikisi de aynı nesneye bakıyor)
```

---

## Özet Karşılaştırma Tablosu

| Özellik | Değer Tipleri (Value Types) | Referans Tipleri (Reference Types) |
| :--- | :--- | :--- |
| **Hafıza Bölgesi** | Stack | Heap (Adresi Stack'te) |
| **Kopyalama Davranışı** | Verinin kendisi kopyalanır, değişkenler bağımsızdır. | Sadece adres kopyalanır, aynı veriyi paylaşırlar. |
| **Varsayılan Değer** | Sayılar için `0`, bool için `false` vb. | `null` (Hiçbir yeri göstermiyor) |
| **Performans** | Küçük ölçekli veriler için oldukça hızlıdır. | Büyük ve karmaşık nesneleri yönetmek için idealdir. |

---

## Bir İstisna: `string` Davranışı

`string` türü yapısal olarak bir **referans tipidir**. Ancak C#'ta geliştirme yaparken kafa karışıklığı yaratmaması adına, tıpkı bir değer tipiymiş gibi davranacak şekilde özel olarak tasarlanmıştır. Bu duruma **immutable (değiştirilemez)** denir. 

Bir `string` değişkenin değerini değiştirdiğinizde, mevcut veri üzerinde düzenleme yapılmaz; arka planda Heap bölgesinde tamamen yeni bir `string` nesnesi oluşturulur ve değişkeniniz artık bu yeni adresi göstermeye başlar.