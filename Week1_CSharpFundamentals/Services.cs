namespace Week1_CSharpFundamentals;

// --- 1. YÖNTEM: ESKİ USUL CLASS (Boşuna yazılan onlarca satır kod) ---
public class ProductServiceOld
{
    private readonly string _connectionString;
    private readonly string _logPrefix;

    public ProductServiceOld(string connectionString, string logPrefix)
    {
        _connectionString = connectionString;
        _logPrefix = logPrefix;
    }

    public void LogConnection()
    {
        Console.WriteLine($"[{_logPrefix}] Eski servis bağlandı: {_connectionString}");
    }
}


// --- 2. YÖNTEM: MODERN CLASS (Primary Constructor - C# 12+) ---
// Parametreler doğrudan sınıf adının yanında! field tanımlama ve atama derdi bitti.
public class ProductService(string connectionString, string logPrefix)
{
    // 'connectionString' ve 'logPrefix' parametreleri bu sınıfın her yerinde doğrudan erişilebilirdir.
    public void LogConnection()
    {
        Console.WriteLine($"[{logPrefix}] Yeni servis bağlandı: {connectionString}");
    }
}