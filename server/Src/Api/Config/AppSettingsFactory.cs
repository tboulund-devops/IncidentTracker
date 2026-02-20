using Domain.Settings;

namespace Api.Config;

public static class AppSettingsFactory
{
    public static AppSettings Create(IConfiguration config)
    {
        var jwtConfig = config
            .GetSection("Jwt")
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt section is missing or invalid in appsettings.");

        var dbConfig = config
            .GetSection("Database")
            .Get<DbSettings>()
            ?? throw new InvalidOperationException("Database section is missing or invalid in appsettings.");

        var corsConfig = config
            .GetSection("Cors")
            .Get<CorsSettings>()
            ?? throw new InvalidOperationException("Cors section is missing or invalid in appsettings.");
        
        return new AppSettings(corsConfig, jwtConfig, dbConfig);
    }
}