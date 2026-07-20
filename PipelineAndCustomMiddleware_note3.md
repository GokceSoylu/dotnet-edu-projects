# HTTP Request Pipeline ve Custom Middleware

## Pipeline (Boru Hattı) Nedir?
Uygulamaya gelen her HTTP isteği, istemciye yanıt (Response) dönene kadar bir zincir halinde sıralanmış yazılım katmanlarından (Middleware) geçer. Bu katmanların dizilim sırası hayati önem taşır; çünkü istek sırayla girer, en son noktaya ulaşır ve aynı sırayla tersten yanıt olarak geri döner.

## Temel Middleware Metotları
.NET içerisinde boru hattını şekillendirmek için 3 temel metot kullanılır:

1. **`Use` (Devam Ettiren):** Gelen isteği işler, bir sonraki middleware'e devreder (`next()`) ve dönüşte yanıtı tekrar işleyebilir. En yaygın kullanılanıdır.
2. **`Run` (Kestirip Atan / Terminal):** İstegi işler ve boru hattını orada bitirir. Kendisinden sonraki hiçbir middleware çalışmaz.
3. **`Map` (Yola Göre Araya Giren):** İstegin atıldığı URL yoluna (Route) göre boru hattını dallandırır.

---

## Mantığı
Bir gece kulübünün kapısındaki güvenlik zinciri gibi düşün:
1. **1. Güvenlik (Authentication Middleware):** "Kimliğin var mı?" Bakar, varsa içeri alır (`next()`).
2. **2. Güvenlik (Logging Middleware):** "İçeri kim girdi, saat kaçta girdi?" Not tutar (`next()`).
3. **Mekan İçi (Endpoint/Controller):** Siparişini verirsin, yemeğini yersin.
4. **Çıkış:** Çıkarken 2. güvenlik tekrar kronometreye bakar: "Bu adam içeride 45 dakika kaldı" der ve kaydeder.