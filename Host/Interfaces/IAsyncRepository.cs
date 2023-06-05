using Models;

namespace Host.Interfaces;

public interface IAsyncRepository<T> where T : Entity
{
    Task<T?> FindAsync (object id);

    Task<T> AddAsync(T entity);

    Task<IEnumerable<T>> GetAsync();

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}