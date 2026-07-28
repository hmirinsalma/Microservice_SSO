using System.Linq.Expressions;

namespace ONEE.SSO.Application.Interfaces.Repositories;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);

    Task<IEnumerable<TEntity>> GetAllAsync();

    Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate);

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task<bool> ExistsAsync(Guid id);

    Task<int> SaveChangesAsync();
}