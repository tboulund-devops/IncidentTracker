namespace Application.Common.Interfaces;

public interface IJwt
{
    string GenerateToken(Guid userId, string email, string role);
}