using Domain.Interfaces.Utility;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Utils;

public sealed class CookieHelper(IEnvHelper envHelper)
{
    private readonly IEnvHelper _envHelper = envHelper;
    public static CookieOptions CreateCookieOptions(TimeSpan maxAge)
    {
        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        
        return new CookieOptions
        {
            HttpOnly = true,
            MaxAge = maxAge,
            Path = "/",
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None
        };
    }
    
    public static CookieOptions CreateAccessTokenCookieOptions(int expirationMinutes)
    {
        return CreateCookieOptions(TimeSpan.FromMinutes(expirationMinutes));
    }
    
    public static CookieOptions CreateRefreshTokenCookieOptions(int expirationDays)
    {
        return CreateCookieOptions(TimeSpan.FromDays(expirationDays));
    }
    
    public static CookieOptions CreateExpiredCookieOptions()
    {
        return CreateCookieOptions(TimeSpan.Zero);
    }
}
