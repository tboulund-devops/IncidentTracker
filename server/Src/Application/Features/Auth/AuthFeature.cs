using Application.Common.Interfaces;
using Application.Common.Interfaces.Features;
using Application.Common.Results;
using Application.DTOs.Responses;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;


namespace Application.Features.Auth;

public class AuthFeature(LoginHandler loginHandler, RegisterUserHandler registerUserHandler) : IAuthFeature
{

    public Task<Result<LoginResponseDto>> HandleLogin(LoginCommand command)
    {
        return loginHandler.HandleAsync(command);
    }

    public Task<Result> HandleRegisterUser(RegisterUserCommand command)
    {
        return registerUserHandler.HandleAsync(command);
    }
}