# Week 2: Base API Template & Financial Wallet Prototype

Bu proje, **.NET İleri Seviye Yol Haritası** kapsamında 2. Haftada öğrenilen tüm temel mimari bileşenlerin, modüler ve kurumsal bir "Base Template" (Temel API Şablonu) üzerinde harmanlanmış halidir.

## 🎯 Projenin Amacı
İleride geliştirilecek olan **Clean Architecture** tabanlı Finansal Cüzdan API'si öncesinde; HTTP istek boru hattını (Pipeline), merkezi hata yönetimini, tip güvenli yanıt yapılarını ve konfigürasyon yönetimini tek bir API şablonunda pekiştirmektir.

---

## 🏗️ Kullanılan Mimari Bileşenler & Teknolojiler

### 1. Minimal API & Controller Hibrit Yapısı
- **Minimal APIs:** Yüksek performans gerektiren bakiye sorgulama ve hızlı transfer uç noktalarında tercih edildi.
- **Controllers:** Yönetimsel ve genişletilebilir raporlama uç noktaları için yapılandırıldı.

### 2. Custom & Global Exception Handling Middleware
- `RequestLoggingMiddleware`: Gelen her isteğin metodunu, yolunu ve milisaniye cinsinden işlenme süresini loglar.
- `ExceptionHandlingMiddleware`: Uygulamanın herhangi bir yerinde fırlatılan `NotFoundException` veya `InsufficientBalanceException` gibi hataları en dışta yakalar ve istemciye **RFC 7807** uyumlu / standart JSON formatında yanıt döner.

### 3. Dependency Injection (DI) Ömürleri
- **`Transient`:** Her istekte yeni Guid / İşlem ID üreten bileşenler.
- **`Scoped`:** İstek bazlı bakiye ve cüzdan işlemlerini yürüten servisler (`IWalletService`).
- **`Singleton`:** Uygulama ömrü boyunca toplam işlem sayısını hafızada tutan sayaç servisleri (`ITransactionCounterService`).

### 4. Options Pattern (`appsettings.json`)
- `WalletSettings` sınıfı ile günlük transfer limitleri ve komisyon oranları `appsettings.json` üzerinden güçlü tipli (strongly-typed) olarak `IOptions<WalletSettings>` ile okunur.

### 5. Type-Safe DTOs & `ApiResponse<T>`
- Veritabanı/Servis varlıklarını gizlemek için `record` yapısındaki DTO'lar kullanıldı.
- `TypedResults` ve `ApiResponse<T>` sarmalayıcısı ile tutarlı JSON çıktıları sağlandı.

---

## 🚀 Endpoint Listesi & Test Senaryoları

| Metot | Rota | Açıklama | Beklenen Durum |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/config/wallet` | `appsettings.json` ayarlarını okur (Options Pattern) | `200 OK` |
| `GET` | `/api/wallet/balance` | Güncel bakiye ve cüzdan bilgilerini getirir | `200 OK` |
| `POST` | `/api/wallet/transfer` | Para transferi gerçekleştirir | `200 OK` / `400 Bad Request` |
| `GET` | `/api/test/notfound` | Özel 404 hatası tetikler | `404 Not Found` |
| `GET` | `/api/test/error` | Beklenmeyen 500 hatası tetikler | `500 Internal Server Error` |