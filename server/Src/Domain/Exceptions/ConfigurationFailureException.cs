namespace Domain.Exceptions;

public class ConfigurationFailureException : Exception
{
    public ConfigurationFailureException() : base() {}
    public ConfigurationFailureException(string message) : base(message) {}
    public ConfigurationFailureException(string message, Exception innerException) : base(message, innerException) {}
}