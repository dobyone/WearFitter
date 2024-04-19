using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using WearFitter.Domain.Common.Query;

namespace WearFitter.EFCore.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<QueryResult<T>> ToQueryResult<T>(this IQueryable<T> queryable, QueryRequest request)
        where T : class
    {
        if (request.Filter != null)
        {
            queryable = queryable.Where(request.Filter);
        }

        int total = await queryable.CountAsync();

        if (request.OrderBys != null)
        {
            queryable = queryable.OrderBy(request.OrderBys);
        }

        if (request.Skip.HasValue)
        {
            queryable = queryable.Skip(request.Skip.Value);
        }

        if (request.Take.HasValue)
        {
            queryable = queryable.Take(request.Take.Value);
        }

        var data = await queryable.ToListAsync();

        var result = new QueryResult<T>(data, total);

        return result;
    }

    private static Expression GetSimpleFilterExpression<T>(SimpleFilterQuery filter)
        where T : class
    {
        LambdaExpression memberExpression = PropertyAccessorCache<T>.Get(filter.PropertyName);

        if (memberExpression == null)
        {
            throw new InvalidOperationException($"Can't find property with name {filter.PropertyName} in class {typeof(T).FullName}");
        }

        object value;

        if (filter.Condition == FilterCondition.LIKE)
        {
            value = filter.Value;
        }
        else
        {
            value = TypeDescriptor.GetConverter(memberExpression.ReturnType).ConvertFromInvariantString(filter.Value);
        }

        Expression filterExpression;

        switch (filter.Condition)
        {
            case FilterCondition.EQUAL:
                {
                    filterExpression = Expression.Equal(
                        memberExpression.Body,
                        Expression.Constant(value, memberExpression.ReturnType)
                    );

                    break;
                }
            case FilterCondition.NOT_EQUAL:
                {
                    filterExpression = Expression.NotEqual(
                        memberExpression.Body,
                        Expression.Constant(value, memberExpression.ReturnType)
                    );

                    break;
                }
            case FilterCondition.LESS_THAN:
                {
                    filterExpression = Expression.LessThan(
                        memberExpression.Body,
                        Expression.Constant(value, memberExpression.ReturnType)
                    );

                    break;
                }
            case FilterCondition.LESS_THAN_OR_EQUAL_TO:
                {
                    filterExpression = Expression.LessThanOrEqual(
                        memberExpression.Body,
                        Expression.Constant(value, memberExpression.ReturnType)
                    );

                    break;
                }
            case FilterCondition.GREATER_THAN:
                {
                    filterExpression = Expression.GreaterThan(
                        memberExpression.Body,
                        Expression.Constant(value, memberExpression.ReturnType)
                    );

                    break;
                }
            case FilterCondition.GREATER_THAN_OR_EQUAL_TO:
                {
                    filterExpression = Expression.GreaterThanOrEqual(
                        memberExpression.Body,
                        Expression.Constant(value, memberExpression.ReturnType)
                    );

                    break;
                }
            case FilterCondition.LIKE:
                {
                    Expression keyExpression = memberExpression.Body;

                    //https://entityframeworkcore.com/knowledge-base/56715091/ef-core-sql-function-like-method-with-linq-expression-not-working-for-non-string-types
                    if (keyExpression.Type != typeof(string))
                    {
                        keyExpression = Expression.Convert(keyExpression, typeof(object));
                        keyExpression = Expression.Convert(keyExpression, typeof(string));
                    }

                    var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                        "Like",
                        new[] { typeof(DbFunctions), typeof(string), typeof(string) }
                    );

                    filterExpression = Expression.Call(
                        null,
                        likeMethod,
                        Expression.Constant(EF.Functions),
                        keyExpression,
                        Expression.Constant($"%{filter.Value}%")
                    );

                    break;
                }
            default:
                throw new InvalidOperationException($"Unknown filter condition {filter.Condition}");
        }

        return filterExpression;
    }

    private static Expression GetComplexFilterExpression<T>(ComplexFilterQuery query)
        where T : class
    {
        Func<Expression, Expression, FilterOperator, Expression> combineExpressions = (left, right, op) =>
        {
            var result = op == FilterOperator.AND
                ? Expression.AndAlso(left, right)
                : Expression.OrElse(left, right);

            return result;
        };

        Expression result = null;

        if ((query.Simple?.Count ?? 0) + (query.Complex?.Count ?? 0) > 1 && !query.Operator.HasValue)
        {
            throw new InvalidOperationException($"Query with more that 1 filter should have operator.");
        }

        if (query.Simple != null)
        {
            foreach (SimpleFilterQuery s in query.Simple)
            {
                Expression simpleFilterExpression = GetSimpleFilterExpression<T>(s);

                result = result != null
                    ? combineExpressions(result, simpleFilterExpression, query.Operator.Value)
                    : simpleFilterExpression;
            }
        }

        if (query.Complex != null)
        {
            foreach (ComplexFilterQuery c in query.Complex)
            {
                Expression complexFilterExpression = GetComplexFilterExpression<T>(c);

                result = result != null
                    ? combineExpressions(result, complexFilterExpression, query.Operator.Value)
                    : complexFilterExpression;
            }
        }

        return result;
    }

    public static IQueryable<T> Where<T>(this IQueryable<T> source, ComplexFilterQuery query) where T : class
    {
        var filterExpression = GetComplexFilterExpression<T>(query);

        if (filterExpression != null)
        {
            var parameter = PropertyAccessorCache<T>.GetParameter();
            var lambdaExpression = Expression.Lambda(filterExpression, parameter);

            MethodCallExpression resultExpression = Expression.Call(
                null,
                GetMethodInfo<IQueryable<T>, Expression<Func<T, bool>>, IQueryable<T>>(Queryable.Where),
                new Expression[] { source.Expression, Expression.Quote(lambdaExpression) }
            );

            return source.Provider.CreateQuery<T>(resultExpression);
        }
        else
        {
            return source;
        }
    }

    public static IQueryable<T> ConditionalExpression<T>(this IQueryable<T> q, Func<IQueryable<T>, IQueryable<T>> expr, bool condition)
    {
        if (condition)
        {
            return expr(q);
        }

        return q;
    }

    private static MethodInfo GetMethodInfo<T1, T2, T3>(
        Func<T1, T2, T3> f)
    {
        return f.Method;
    }

    private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, ICollection<OrderQuery> query) where T : class
    {
        Func<OrderDirection, string> getFistOrder = (direction) =>
        {
            return direction == OrderDirection.Ascending
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);
        };

        Func<OrderDirection, string> getNextOrder = (direction) =>
        {
            return direction == OrderDirection.Ascending
                ? nameof(Queryable.ThenBy)
                : nameof(Queryable.ThenByDescending);
        };

        Type type = typeof(T);
        bool isFirst = true;

        foreach (var order in query)
        {
            LambdaExpression expression = PropertyAccessorCache<T>.Get(order.PropertyName);

            if (expression == null)
            {
                throw new InvalidOperationException($"Can't find property with name {order.PropertyName} in class {type.FullName}");
            }

            string methodName = isFirst
                ? getFistOrder(order.Direction)
                : getNextOrder(order.Direction);

            MethodCallExpression resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), expression.ReturnType },
                source.Expression,
                Expression.Quote(expression)
            );

            source = source.Provider.CreateQuery<T>(resultExpression);

            isFirst = false;
        }

        return source;
    }

    private class PropertyNameEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
            => string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;

        public int GetHashCode([DisallowNull] string obj)
            => obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    private static class PropertyAccessorCache<T> where T : class
    {
#pragma warning disable S2743 // Static fields should not be used in generic types
        private static readonly IDictionary<string, LambdaExpression> cache;
#pragma warning restore S2743 // Static fields should not be used in generic types

#pragma warning disable S3963 // "static" fields should be initialized inline
        static PropertyAccessorCache()
        {
            var storage = new Dictionary<string, LambdaExpression>(new PropertyNameEqualityComparer());
            var t = typeof(T);
            var parameter = Expression.Parameter(t, "item");

            foreach (var property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var lambdaExpression = Expression.Lambda(propertyAccess, parameter);
                storage[property.Name] = lambdaExpression;
            }

            cache = storage;
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

        public static LambdaExpression Get(string propertyName)
        {
            LambdaExpression result;
            return cache.TryGetValue(propertyName, out result) ? result : null;
        }

        public static ParameterExpression GetParameter()
        {
            return cache.First().Value.Parameters[0];
        }
    }
}
