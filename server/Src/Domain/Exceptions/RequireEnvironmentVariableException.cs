namespace Domain.Exceptions;

public class RequireEnvironmentVariableException : Exception
{
    public RequireEnvironmentVariableException() : base() {}
    public RequireEnvironmentVariableException(string message) : base(message) {}
    public RequireEnvironmentVariableException(string message, Exception inner) : base(message, inner) {}
    
}