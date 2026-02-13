using System.ComponentModel.DataAnnotations;

namespace Domain.Settings;

public sealed class DbSettings()
{
    public const string SectionName = "Database";
    public required string PSqlConnectionString { get; init; }
    public required string PSqlUser { get; init; }
    public required string PSqlPassword { get; init; }
    public required string DbName { get; init; }
}