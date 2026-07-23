namespace Week2_BaseApiTemplate.Exceptions;

public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException(string message) : base(message) { }
}