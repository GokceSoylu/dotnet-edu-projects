# Global Exception Handling (Küresel Hata Yönetimi)

## Neden `try-catch` Kalabalığından Kaçınmalıyız?
Her Controller veya Minimal API endpoint'inin içine ayrı ayrı `try-catch` blokları yazmak **DRY (Don't Repeat Yourself)** prensibine aykırıdır. Kod kalabalığı yaratır ve gözden kaçan bir noktada uygulamanın patlamasına sebep olur.

## Merkezi Hata Yönetimi Mimarisi
İstek boru hattının (pipeline) en başına bir `ExceptionHandlingMiddleware` yerleştirilir. Bu middleware, kendisinden sonraki tüm middleware ve endpoint çağrılarını bir `try-catch` bloğu içine alır:

1. İstek normal bir şekilde işlenirse middleware müdahale etmez.
2. Boru hattının herhangi bir derinliğinde bir hata (`Exception`) fırlatılırsa, istek geriye doğru akarken bu middleware hatayı yakalar (catch).
3. Hatayı loglar ve istemciye (frontend) teknik detayları gizlenmiş, standardize edilmiş bir JSON yanıtı oluşturup döndürür.

## Mantığı
Bir restorandasın dostum. Masada su bardağı kırıldı ya da mutfakta bir tabak düştü. Her müşterinin başına bir garson dikip "Sakın korkmayın" demek yerine, restoranda bir **Güvenlik/Kriz Yönetim Prosedürü** vardır. Bir şey patladığı an bu mekanizma devreye girer; müşteriye çaktırmadan salonu temizler, özür diler ve masaya standart bir bilgilendirme notu bırakır.