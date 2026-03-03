using Domain.Entities;

namespace Application.DTOs.Entities;

public sealed record UserDto()
{
    public string? Username { get; init; }
    public string? Email { get; init; }

    public static UserDto? Map(User? user)
    {
        return user == null
            ? null
            : new UserDto
            {
                Email = user.Email,
                Username = $"{user.FirstName} {user.LastName}".Trim()
            };
    }
}