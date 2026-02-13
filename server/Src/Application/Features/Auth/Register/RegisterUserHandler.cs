using Application.Common.Interfaces;
using Application.Common.Results;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utility;

namespace Application.Features.Auth.Register;

public sealed class RegisterUserHandler(
    IUserRepository userRepository,
    IHashingUtils hashingUtils
) : ICommandHandler<RegisterUserCommand, Result>
{
    public async Task<Result> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if a user with this email already exists
            var existingUser = await GetExistingUser(command.Email);
            if (existingUser is not null)
                return Result.Failed("A user with this email already exists.");

            hashingUtils.CreatePasswordHash(command.Password, out var passwordHash);

            var user = User.Create(
                command.FirstName,
                command.LastName,
                command.Email,
                passwordHash,
                command.Role,
                command.PhoneNumber
            );

            await userRepository.AddAsync(user);

            return Result.Success("User registered successfully.");
        }
        catch (RepositoryException e)
        {
            return Result.Failed(e.Message);
        }
    }

    private async Task<User?> GetExistingUser(string email)
    {
        try
        {
            return await userRepository.GetByEmailAsync(email);
        }
        catch (EntityNotFoundException)
        {
            return null;
        }
    }
}