namespace Week2_MinimalVsController.DTOs;

// İstemciye her zaman standart bir JSON şablonu dönmek için kullanılan sarmalayıcı (wrapper)
public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Success(T data, string message = "İşlem başarılı.")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = default
        };
    }
}

// Dış dünyaya açılacak temiz veri modeli (Veritabanı varlığının DTO hali)
public record ProductDto(int Id, string Name, decimal Price);