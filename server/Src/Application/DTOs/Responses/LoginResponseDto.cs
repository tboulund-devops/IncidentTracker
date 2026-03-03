using Application.DTOs.Entities;

namespace Application.DTOs.Responses;

public sealed record LoginResponseDto
{
    public required string AccessToken { get; init; }
    public required UserDto User { get; init; }
    public string? RefreshToken { get; init; }
}