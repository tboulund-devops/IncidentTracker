using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User> GetByEmailAsync(string email);
}