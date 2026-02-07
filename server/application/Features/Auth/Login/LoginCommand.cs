using application.Common;

namespace application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password);
