using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public Task<User> AddAsync(User entity)
    {
        throw new NotImplementedException();
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