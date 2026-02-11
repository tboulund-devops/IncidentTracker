namespace Domain.Interfaces.Utility;

public interface IHashingUtils
{
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string GenerateRefreshToken();
}