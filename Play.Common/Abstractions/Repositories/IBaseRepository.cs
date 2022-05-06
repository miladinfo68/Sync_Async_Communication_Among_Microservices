using Play.Common.Entities;
using System.Linq.Expressions;

namespace Play.Common.Abstractions.Repositories;
public interface IBaseRepository<T> where T : IEntity
{
    Task<IReadOnlyCollection<T>> GetAll(Expression<Func<T, bool>> filter = null);
    Task<T> GetBy(Expression<Func<T, bool>> filter);
    Task<T> Get(Guid id);
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(Guid id);
}
