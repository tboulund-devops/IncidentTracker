namespace Domain.Settings;

public sealed class AppSettings(CorsSettings corsSettings, JwtSettings jwtSettings, DbSettings dbSettings)
{
    public CorsSettings CorsSettings { get; } = corsSettings;
    public JwtSettings JwtSettings { get; } = jwtSettings;
    public DbSettings DbSettings { get; } = dbSettings;
}