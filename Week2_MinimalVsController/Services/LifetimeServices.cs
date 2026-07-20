namespace Week2_MinimalVsController.Services;

// 1. Arayüzler (Interfaces)
public interface ITransientService
{
    Guid GetGuid();
}

public interface IScopedService
{
    Guid GetGuid();
}

public interface ISingletonService
{
    Guid GetGuid();
}

// 2. Uygulama Sınıfı (Tek sınıf 3 arayüzü de uygulayabilir veya ayrı ayrı yazılabilir)
public class LifetimeService : ITransientService, IScopedService, ISingletonService
{
    private readonly Guid _guid;

    public LifetimeService()
    {
        // Nesne hafızada ilk defa oluşturulduğu an benzersiz bir ID üretir
        _guid = Guid.NewGuid();
    }

    public Guid GetGuid() => _guid;
}