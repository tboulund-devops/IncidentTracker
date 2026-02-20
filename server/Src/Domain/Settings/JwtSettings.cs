using System.Text;
using Domain.Exceptions;

namespace Domain.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required uint AccessTokenLifetime { get; init; }
    public required uint RefreshTokenLifetime { get; init; }

    private const int MinSecretLenghtBytes = 32; // 256 bits for HS256
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Secret))
            throw new ConfigurationFailureException("JWT Secret is missing.");
        if (Encoding.UTF8.GetByteCount(Secret) < MinSecretLenghtBytes)
            throw new ConfigurationFailureException(
                $"JWT Secret must be at least {MinSecretLenghtBytes} bytes (got {Encoding.UTF8.GetByteCount(Secret)}).");
    }
}