using Domain.Enums;

namespace Application.Features.Auth.Register;

public sealed record RegisterUserCommand
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public RoleType Role { get; init; } = RoleType.User;
}