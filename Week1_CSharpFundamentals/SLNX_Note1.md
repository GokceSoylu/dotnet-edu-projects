### DotNetCoreBootcamp.slnx (Solution Dosyası)

*   **Mantığı:** Solution (Çözüm), bir çatıdır. Kendisi bir kod barındırmaz. Sadece projedeki alt klasörleri ve `.csproj` (proje) dosyalarını tek bir çatı altında toplamaya yarar. Eskiden `.sln` formatındaydı; yeni .NET sürümlerinde XML tabanlı ve çok daha hafif olan yeni nesil `.slnx` formatı kullanılıyor.
*   **Nasıl Bağlanır?** Bu dosyanın içine sağ tıklayıp "Add Existing Project" dediğinde ya da terminalden komut verdiğinde, `Week1_CSharpFundamentals.csproj` dosyasının yolunu kendi içine kaydeder. Böylece IDE'yi (VS Code / Rider / Visual Studio) bu `.slnx` üzerinden açtığında tüm projelerini bir arada görürsün.

### Week1_CSharpFundamentals (Klasör ve .csproj)

*   **Mantığı:** Bu senin asıl çalıştırılabilir programındır (büyük ihtimalle bir Console Application). İçindeki `.csproj` dosyası, bu projenin hangi .NET versiyonunu kullandığını (örn: .NET 8 veya .NET 9) ve hangi dış paketlere (NuGet) bağımlı olduğunu tutar.

### Program.cs

*   **Mantığı:** Uygulamanın giriş kapısıdır (Entry Point). Kodunu çalıştırdığında .NET ilk olarak bu dosyanın içindeki kodları yukarıdan aşağıya doğru yürütür.

### bin ve obj Klasörleri

*   **Mantığı:** Bunlar otomatik oluşur. Kodunu derlediğinde (build ettiğinde) makine diline dönüşen çalıştırılabilir dosyalar (`.dll`, `.exe`) `bin` klasörünün içine çıkar. Bunlara dokunmana gerek yoktur.