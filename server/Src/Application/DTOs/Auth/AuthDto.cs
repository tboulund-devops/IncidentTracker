using Application.DTOs.Entities;

namespace Application.DTOs.Auth;

public sealed record AuthDto
{
    public required UserDto UserDto { get; init; }
    public required string AccessToken { get; init; }
}

