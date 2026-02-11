using Application.Common.Interfaces;

namespace Infrastructure.Auth;

public class Jwt : IJwt
{
    public string GenerateToken(Guid userId, string email, string role)
    {
        // TODO: Implement JWT token generation using JwtSettings
        throw new NotImplementedException();
    }
}