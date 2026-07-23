namespace Week2_BaseApiTemplate.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}