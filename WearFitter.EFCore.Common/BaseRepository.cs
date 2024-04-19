using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WearFitter.Domain.Common;
using WearFitter.Domain.Common.Query;
using WearFitter.EFCore.Common.Extensions;

namespace WearFitter.EFCore.Common;

public abstract class BaseRepository<TContext, T>(TContext dbContext) :
    IRepository<T>
    where TContext : DbContext
    where T : class
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    protected TContext DbContext { get; } = dbContext;

    public Task<T> Get(params object[] keys)
        => Get(string.Empty, keys);

    public async Task<T> Get(string includeProperties = "", params object[] keys)
    {
        var lambda = BuildPrimaryKeyLambda(keys);

        T result = await _dbSet
            .AsNoTracking()
            .With(includeProperties)
            .SingleOrDefaultAsync(lambda);

        return result;
    }

    public async Task<IEnumerable<T>> GetAll(string includeProperties = "")
    {
        List<T> result = await _dbSet
            .AsNoTracking()
            .With(includeProperties)
            .ToListAsync();

        return result;
    }

    public async Task<QueryResult<T>> GetAllByFilter(QueryRequest request, string includeProperties = "")
    {
        QueryResult<T> result = await _dbSet
            .AsNoTracking()
            .With(includeProperties)
            .ToQueryResult(request);

        return result;
    }

    public async Task<IEnumerable<T>> GetAllByFilter(Expression<Func<T, bool>> predicate, string includeProperties = "")
    {
        List<T> result = await _dbSet
            .AsNoTracking()
            .With(includeProperties)
            .Where(predicate)
            .ToListAsync();

        return result;
    }

    public async Task Create(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task CreateMany(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public Task Update(T entity)
    {
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(T entity)
    {
        if (DbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);

        return Task.CompletedTask;
    }

    public async Task Delete(params object[] keys)
    {
        T entity = await Get(keys);
        await Delete(entity);
    }

    public Task DeleteRange(ComplexFilterQuery filter)
    {
        IQueryable<T> entities = _dbSet.Where(filter);

        DbContext.RemoveRange(entities);

        return Task.CompletedTask;
    }

    public Task DeleteRange(Expression<Func<T, bool>> predicate)
    {
        var entities = _dbSet.Where(predicate);

        _dbSet.RemoveRange(entities);

        return Task.CompletedTask;
    }

    private Expression<Func<T, bool>> BuildPrimaryKeyLambda(params object[] keys)
    {
        List<string> keyProperties = DbContext.Model
           .FindEntityType(typeof(T))
           .FindPrimaryKey().Properties
           .Select(p => p.Name)
           .ToList();

        if (keyProperties.Count != keys.Count())
        {
            throw new ArgumentException("Count of primary key properties should match provided values count.");
        }

        var parameter = Expression.Parameter(typeof(T), "e");

        Expression predicate = BuildEqualsExpression(parameter, keyProperties[0], keys[0]);

        for (var i = 1; i < keyProperties.Count; i++)
        {
            predicate = Expression.AndAlso(predicate, BuildEqualsExpression(parameter, keyProperties[i], keys[i]));
        }

        return Expression.Lambda<Func<T, bool>>(predicate, parameter);
    }

    private Expression BuildEqualsExpression(Expression parameter, string propertyName, object value) =>
        Expression.Equal(
            Expression.PropertyOrField(parameter, propertyName),
            Expression.Constant(value)
        );
}

internal static class QueryableExtensions
{
    public static IQueryable<T> With<T>(this IQueryable<T> query, string includeProperties) where T : class
    {
        if (string.IsNullOrWhiteSpace(includeProperties))
        {
            return query;
        }

        char[] separators = [','];

        foreach (var includeProperty in includeProperties.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        return query;
    }
}
