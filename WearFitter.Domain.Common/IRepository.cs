using System.Linq.Expressions;
using WearFitter.Domain.Common.Query;

namespace WearFitter.Domain.Common;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(string includeProperties = "");

    Task<QueryResult<T>> GetAllByFilter(QueryRequest request, string includeProperties = "");

    Task<IEnumerable<T>> GetAllByFilter(Expression<Func<T, bool>> predicate, string includeProperties = "");

    Task<T> Get(params object[] keys);

    Task<T> Get(string includeProperties = "", params object[] keys);

    Task Create(T entity);

    Task CreateMany(IEnumerable<T> entities);

    Task Update(T entity);

    Task Delete(T entity);

    Task Delete(params object[] keys);

    Task DeleteRange(ComplexFilterQuery filter);

    Task DeleteRange(Expression<Func<T, bool>> predicate);
}
