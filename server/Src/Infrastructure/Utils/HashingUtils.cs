using System.Security.Cryptography;
using System.Text;
using Domain.Interfaces.Utility;

namespace Infrastructure.Utils;

public class HashingUtils : IHashingUtils
{
    public void CreatePasswordHash(string password, out byte[] passwordHash)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 13);
        passwordHash = Encoding.UTF8.GetBytes(hash);
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash)
    {
        var hash = Encoding.UTF8.GetString(passwordHash);
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GenerateRefreshToken()
    {
        var randomNr = new byte[64];
        RandomNumberGenerator.Fill(randomNr);
        return Convert.ToBase64String(randomNr);
    }
}