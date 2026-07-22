# Data Transfer Objects (DTOs) ve Standardized API Responses

## Neden DTO Kullanmalıyız?
Veritabanı modellerimizi (Entities) doğrudan API yanıtı olarak dışarıya açmak iki büyük problem yaratır:
1. **Güvenlik Açığı (Over-posting / Data Leakage):** Kullanıcının görmemesi gereken şifre hash'i, iç kimlik numaraları veya silinmiş işareti (IsDeleted) gibi alanlar dışarı sızabilir.
2. **Sıkı Bağımlılık (Tight Coupling):** Veritabanı tablosunda yapılan bir sütun değişikliği doğrudan API sözleşmesini (contract) bozar ve istemcileri patlatır.

DTO'lar, veritabanı modeli ile dış dünya arasına bir kalkan koyarak sadece ihtiyaç duyulan verilerin iletilmesini sağlar.

## `IResult` vs `TypedResults` (Minimal API)
* **`IResult`:** Minimal API endpoint'lerinde esnek yanıt dönmeyi sağlar (`Results.Ok()`, `Results.NotFound()`). Ancak geri dönüş tipi sözel olarak bildirilmediği için OpenAPI/Swagger dokümantasyonunda otomatik tip çıkarımı zayıftır.
* **`TypedResults`:** Tip güvenliği (Type Safety) sunar. Endpoint'in tam olarak hangi tipleri dönebileceğini derleme zamanında (compile-time) kontrol eder ve birim testlerini (unit test) yazmayı inanılmaz kolaylaştırır.

## Kısaca Mantığı
Bir restoranda mutfaktaki devasa malzeme çuvallarını masaya getirmek yerine, garsonun yemeği tabağa estetikçe dizip sunması gibidir. Mutfaktaki orijinal çuval veritabanı varlığındır (`Entity`), garsonun getirdiği tabak ise `DTO`'dur. İstemci sadece tabağı görür, mutfağın iç düzenini bilmez.