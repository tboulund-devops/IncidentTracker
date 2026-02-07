using application.Common;

namespace application.Features.Auth.Login;

public sealed class LoginHandler : IUseFeature
{
    public async Task<string> Handle(LoginCommand request)
    {
        return request.Email;
    }
}