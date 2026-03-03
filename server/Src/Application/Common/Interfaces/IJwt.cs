namespace Application.Common.Interfaces;

public interface IJwt
{
    Task<string> GenerateToken(Guid userId);
    
    /// <summary>
    /// Validates the refresh token and generates new tokens.
    /// </summary>
    /// <returns>A tuple containing the new access token and rotated refresh token.</returns>
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken, Guid userId);
}