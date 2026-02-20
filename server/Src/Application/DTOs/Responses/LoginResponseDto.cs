using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.DTOs.Responses;

public sealed record LoginResponseDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required UserDto? User { get; init; }
    
}