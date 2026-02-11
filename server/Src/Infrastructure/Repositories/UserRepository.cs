using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(MyDbContext dbContext) : IUserRepository
{
    public async Task<User> AddAsync(User entity)
    {
        try
        {
            var createdEntity = await dbContext.Users.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return createdEntity.Entity;
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, e);
        }
        catch (OperationCanceledException e)
        {
            throw new RepositoryException(e.Message, e);
        }
    }

    public Task<bool> DeleteAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<User> FindByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}