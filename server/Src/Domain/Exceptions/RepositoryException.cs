namespace Domain.Exceptions;

public class RepositoryException : Exception
{
    public RepositoryException() { }
    public RepositoryException(string message) : base(message) { }
    public RepositoryException(string message, Exception inner) : base(message, inner) { }
}