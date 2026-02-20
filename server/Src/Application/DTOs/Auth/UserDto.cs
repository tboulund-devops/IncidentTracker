namespace Application.DTOs.Auth;

public sealed record UserDto()
{
    public string? Username { get; init; }
    public string? Email { get; init; }
}