namespace application.Features.Auth.Register;

public sealed record RegisterUserCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required DateOnly BirthDate { get; init; }
}