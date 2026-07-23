namespace Week2_BaseApiTemplate.DTOs;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Success(T data, string message = "İşlem başarılı.")
    {
        return new ApiResponse<T> { IsSuccess = true, Data = data, Message = message };
    }

    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T> { IsSuccess = false, Data = default, Message = message };
    }
}