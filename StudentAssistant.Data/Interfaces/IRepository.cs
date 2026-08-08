using System.Linq.Expressions;
using StudentAssistant.Domain.Common;

namespace StudentAssistant.Data.Interfaces;

public interface IRepository<TEntity> where TEntity : Auditable
{
    Task<TEntity?> GetByIdAsync(long id);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
    IQueryable<TEntity> SelectAll(Expression<Func<TEntity, bool>>? predicate = null);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> AddAsync(TEntity entity);
    TEntity Update(TEntity entity);
    TEntity Delete(TEntity entity);
    Task<int> SaveChangesAsync();
}
