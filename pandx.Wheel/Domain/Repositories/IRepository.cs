using System.Linq.Expressions;
using pandx.Wheel.Domain.Entities;

namespace pandx.Wheel.Domain.Repositories;

public interface IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
{
    IQueryable<TEntity> GetAll();
    Task<IQueryable<TEntity>> GetAllAsync();

    IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
    Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);
    Task<IQueryable<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] propertySelectors);

    List<TEntity> GetAllList();
    Task<List<TEntity>> GetAllListAsync();

    List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

    T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

    TEntity Get(TPrimaryKey id);
    Task<TEntity> GetAsync(TPrimaryKey id);

    TEntity Single(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity? FirstOrDefault(TPrimaryKey id);
    Task<TEntity?> FirstOrDefaultAsync(TPrimaryKey id);

    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity Insert(TEntity entity);
    Task<TEntity> InsertAsync(TEntity entity);

    TPrimaryKey InsertAndGetId(TEntity entity);
    Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

    TEntity Update(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);

    TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);
    Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction);

    void Delete(TEntity entity);
    Task DeleteAsync(TEntity entity);

    void Delete(TPrimaryKey id);
    Task DeleteAsync(TPrimaryKey id);

    void Delete(Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    int Count();
    int Count(Expression<Func<TEntity, bool>> predicate);

    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

    Task<long> LongCountAsync();
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
}

public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
{
}