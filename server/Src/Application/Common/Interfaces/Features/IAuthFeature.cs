using Application.Common.Results;
using Application.DTOs.Entities;
using Application.DTOs.Responses;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;

namespace Application.Common.Interfaces.Features;

public interface IAuthFeature
{
    Task<Result<LoginResponseDto>> HandleLogin(LoginCommand command);
    Task<Result> HandleRegisterUser(RegisterUserCommand command);
    Task<Result<UserDto>> HandleMeRequest(Guid myId);
    Task<Result> RevokeRefreshToken(Guid userId);
}