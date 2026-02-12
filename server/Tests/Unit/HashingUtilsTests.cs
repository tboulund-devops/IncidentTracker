using Domain.Interfaces.Utility;
using Infrastructure.Utils;

namespace Unit;

[Collection("Utils Collection")]
public class HashingUtilsTests(IHashingUtils hashingUtils)
{

    [Fact]
    public Task ShouldCreatePasswordHash_AndVerify()
    {
        //Arrange
        var password = "password";
        //Act
        hashingUtils.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        //Assert
        Assert.True(hashingUtils.VerifyPasswordHash(password, passwordHash, passwordSalt));
        
        return Task.CompletedTask;
    }
}