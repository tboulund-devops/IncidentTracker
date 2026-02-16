using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utility;
using Domain.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;


public sealed class Jwt(JwtSettings jwtSettings, IUserRepository userRepository, IHashingUtils hashingUtils): IJwt
{
    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.Name, user.FirstName),
            new (ClaimTypes.Surname, user.LastName),
            new (ClaimTypes.DateOfBirth, user.DateOfBirth.ToString("dd-MM-yyyy")),
            new ("is_active", user.Activated.ToString(CultureInfo.InvariantCulture)),
            new (ClaimTypes.Role, user.Role.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            audience: jwtSettings.Audience,
            issuer: jwtSettings.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenLifetime),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateToken(Guid userId)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(userId);
            return GenerateAccessToken(user);
        }
        catch (RepositoryException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    /// <summary>
    /// Validates the refresh token and generates a new access token.
    /// Only rotates the refresh token if it's close to expiring (less than 25% lifetime remaining).
    /// </summary>
    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken, Guid userId)
    {
        var user = await userRepository.FindByIdAsync(userId);
        
        if (user.RefreshToken != refreshToken)
            throw new AuthenticationException("Invalid refresh token");
        
        if (user.RefreshTokenExpires < DateTime.UtcNow)
            throw new AuthenticationException("Refresh token expired");
        
        // Generate new access token
        var newAccessToken = GenerateAccessToken(user);
        
        // Only rotate refresh token if less than 25% of its lifetime remains
        var refreshTokenLifetimeMinutes = jwtSettings.RefreshTokenLifetime;
        var rotationThreshold = TimeSpan.FromMinutes(refreshTokenLifetimeMinutes * 0.25);
        var timeRemaining = user.RefreshTokenExpires - DateTime.UtcNow;
        
        if (timeRemaining <= rotationThreshold)
        {
            var newRefreshToken = hashingUtils.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(refreshTokenLifetimeMinutes);
            await userRepository.UpdateAsync(user);
            return (newAccessToken, newRefreshToken);
        }
        
        return (newAccessToken, refreshToken);
    }
}
