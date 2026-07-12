namespace Week1_CSharpFundamentals;

public record ProductDto(string Name, decimal Price, string Category);

public readonly record struct GPSCoordinate(double Latitude, double Longitude);