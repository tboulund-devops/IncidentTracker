namespace Domain.Exceptions;

public class WrongTypeEnvironmentVariableException : Exception
{
    public WrongTypeEnvironmentVariableException() : base() {}
    public WrongTypeEnvironmentVariableException(string value) : base(value) {}
    public WrongTypeEnvironmentVariableException(string value, Exception inner) : base(value, inner) {}
    
}