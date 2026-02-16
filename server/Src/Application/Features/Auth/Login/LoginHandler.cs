using System.Security.Authentication;
using Application.Common.Interfaces;
using Application.Common.Results;
using Application.DTOs.Responses;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utility;

namespace Application.Features.Auth.Login;

public sealed class LoginHandler(
    IUserRepository userRepository,
    IJwt jwt,
    IHashingUtils hashingUtils
    ) : ICommandHandler<LoginCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(command.Email);

            if (!hashingUtils.VerifyPasswordHash(command.Password, user.PasswordHash))
                return Result<LoginResponseDto>.Failure("Invalid credentials.", ResultStatus.Unauthorized);

            var token = await jwt.GenerateToken(user.Id);

            var dto = new LoginResponseDto
            {
                Token = token,
                Username = $"{user.FirstName} {user.LastName}"
            };

            return Result<LoginResponseDto>.Success(dto);
        }
        catch (EntityNotFoundException e)
        {
            return Result<LoginResponseDto>.Failure("Invalid credentials.", ResultStatus.Unauthorized);
        }
        catch (RepositoryException e)
        {
            return Result<LoginResponseDto>.Failure(e.Message);
        }
    }
}