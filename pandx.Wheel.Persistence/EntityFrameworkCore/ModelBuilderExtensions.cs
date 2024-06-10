using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace pandx.Wheel.Persistence.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigureGlobalQueryFilter<T>(this ModelBuilder modelBuilder,
        Expression<Func<T, bool>> filter) where T : class
    {
        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(e => e.BaseType is null && e.ClrType.GetInterface(typeof(T).Name) is not null)
            .Select(e => e.ClrType);
        foreach (var entity in entities)
        {
            var parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
            var filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), parameterType, filter.Body);


            if (modelBuilder.Entity(entity).Metadata.GetQueryFilter() is { } existingFilter)
            {
                var existingFilterBody = ReplacingExpressionVisitor.Replace(existingFilter.Parameters.Single(),
                    parameterType, existingFilter.Body);
                filterBody = Expression.AndAlso(existingFilterBody, filterBody);
            }

            modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(filterBody, parameterType));
        }

        return modelBuilder;
    }
}