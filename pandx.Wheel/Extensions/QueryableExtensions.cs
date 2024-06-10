using System.Linq.Expressions;
using pandx.Wheel.Models;

namespace pandx.Wheel.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int currentPage, int pageSize)
    {
        _ = query ?? throw new ArgumentNullException(nameof(query));


        return query.Skip((currentPage - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, PagedRequest request)
    {
        return query.PageBy(request.CurrentPage, request.PageSize);
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }
}