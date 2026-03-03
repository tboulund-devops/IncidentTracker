using System.ComponentModel.DataAnnotations;

namespace Domain.Settings;

public sealed class DbSettings()
{
    public const string SectionName = "Database";
    public required string PSqlConnectionString { get; init; }
    public required string RedisConnectionString { get; init; }
}