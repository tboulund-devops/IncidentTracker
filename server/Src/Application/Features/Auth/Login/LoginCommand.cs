namespace Application.Features.Auth.Login;

public sealed record LoginCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}