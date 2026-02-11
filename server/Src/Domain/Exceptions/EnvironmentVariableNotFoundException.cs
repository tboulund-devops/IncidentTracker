namespace Domain.Exceptions;

public class EnvironmentVariableNotFoundException : Exception
{
    public EnvironmentVariableNotFoundException() : base() {}
    public EnvironmentVariableNotFoundException(string message) : base(message) {}
    public EnvironmentVariableNotFoundException(string message, Exception inner) : base(message, inner) {}
}