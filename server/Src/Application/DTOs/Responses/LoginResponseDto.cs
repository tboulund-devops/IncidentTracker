namespace Application.DTOs.Responses;

public sealed record LoginResponseDto
{
    public required string Token { get; init; }
    public required string Username { get; init; }
}