namespace Week1_CSharpFundamentals;

public readonly record struct CustomerInfo(int Id, String Name, int Age);

public record Customer(String agent, CustomerInfo info);
