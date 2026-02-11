namespace Domain.Interfaces.Repositories;

public interface IBaseRepository<T>
{
    Task<T>  AddAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<T> FindByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<bool> UpdateAsync(T entity);
    
}