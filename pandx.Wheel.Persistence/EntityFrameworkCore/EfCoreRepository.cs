using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.Domain.Entities;
using pandx.Wheel.Domain.Repositories;

namespace pandx.Wheel.Persistence.EntityFrameworkCore;

public abstract class EfCoreRepository<TDbContext, TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey> where TDbContext : DbContext
{
    private readonly TDbContext _context;


    protected EfCoreRepository(TDbContext context)
    {
        _context = context;
    }

    public IQueryable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public Task<IQueryable<TEntity>> GetAllAsync()
    {
        return Task.FromResult(_context.Set<TEntity>().AsQueryable());
    }

    public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().Where(predicate);
    }

    public async Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return (await GetAllAsync()).Where(predicate);
    }

    public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        if (propertySelectors.IsNullOrEmpty())
        {
            return GetAll();
        }

        var query = GetAll();
        foreach (var propertySelector in propertySelectors)
        {
            query = query.Include(propertySelector);
        }

        return query;
    }

    public Task<IQueryable<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return GetAllAsync();
    }

    public List<TEntity> GetAllList()
    {
        return GetAll().ToList();
    }

    public async Task<List<TEntity>> GetAllListAsync()
    {
        return await (await GetAllAsync()).ToListAsync();
    }

    public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().Where(predicate).ToList();
    }

    public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var query = await GetAllAsync();
        return await query.Where(predicate).ToListAsync();
    }

    public T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
    {
        return queryMethod(GetAll());
    }

    public TEntity Get(TPrimaryKey id)
    {
        var entity = FirstOrDefault(id);
        _ = entity ?? throw new Exception($"没有找到类型为 {typeof(TEntity)} 的实体");

        return entity;
    }

    public async Task<TEntity> GetAsync(TPrimaryKey id)
    {
        var entity = await FirstOrDefaultAsync(id);
        if (entity == null)
        {
            throw new Exception($"{typeof(TEntity)} is not found");
        }

        return entity;
    }

    public TEntity Single(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().Single(predicate);
    }

    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var query = await GetAllAsync();
        return await query.SingleAsync(predicate);
    }

    public TEntity? FirstOrDefault(TPrimaryKey id)
    {
        return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
    }

    public async Task<TEntity?> FirstOrDefaultAsync(TPrimaryKey id)
    {
        var query = await GetAllAsync();
        return await query.FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
    }

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().FirstOrDefault(predicate);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var query = await GetAllAsync();
        return await query.FirstOrDefaultAsync(predicate);
    }

    public TEntity Insert(TEntity entity)
    {
        var result = _context.Add(entity);
        return result.Entity;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        var result = await _context.AddAsync(entity);
        return result.Entity;
    }

    public TPrimaryKey InsertAndGetId(TEntity entity)
    {
        entity = Insert(entity);
        _context.SaveChanges();
        return entity.Id;
    }

    public async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
    {
        entity = await InsertAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public TEntity Update(TEntity entity)
    {
        AttachIfNot(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        AttachIfNot(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(entity);
    }

    public TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
    {
        var entity = Get(id);
        updateAction(entity);
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction)
    {
        var entity = await GetAsync(id);
        await updateAction(entity);
        return entity;
    }

    public void Delete(TEntity entity)
    {
        AttachIfNot(entity);
        _context.Set<TEntity>().Remove(entity);
    }

    public Task DeleteAsync(TEntity entity)
    {
        Delete(entity);
        return Task.CompletedTask;
    }

    public void Delete(TPrimaryKey id)
    {
        var entity = _context.Set<TEntity>().Local
            .FirstOrDefault(ent => EqualityComparer<TPrimaryKey>.Default.Equals(ent.Id, id));
        if (entity is null)
        {
            entity = FirstOrDefault(id);
            if (entity is null)
            {
                return;
            }
        }

        Delete(entity);
    }

    public Task DeleteAsync(TPrimaryKey id)
    {
        Delete(id);
        return Task.CompletedTask;
    }

    public void Delete(Expression<Func<TEntity, bool>> predicate)
    {
        foreach (var entity in GetAllList(predicate))
        {
            Delete(entity);
        }
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = await GetAllListAsync(predicate);

        foreach (var entity in entities)
        {
            await DeleteAsync(entity);
        }
    }

    public int Count()
    {
        return GetAll().Count();
    }

    public int Count(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().Count(predicate);
    }

    public async Task<int> CountAsync()
    {
        var query = await GetAllAsync();
        return await query.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var query = (await GetAllAsync()).Where(predicate);
        return await query.CountAsync();
    }

    public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var query = (await GetAllAsync()).Where(predicate);
        return await query.LongCountAsync();
    }

    public async Task<long> LongCountAsync()
    {
        var query = await GetAllAsync();
        return await query.LongCountAsync();
    }

    private void AttachIfNot(TEntity entity)
    {
        if (!_context.Set<TEntity>().Local.Contains(entity))
        {
            _context.Set<TEntity>().Attach(entity);
        }
    }

    private Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
    {
        var lambdaParameter = Expression.Parameter(typeof(TEntity));
        var leftExpression = Expression.PropertyOrField(lambdaParameter, "Id");
        var idValue = Convert.ChangeType(id, typeof(TPrimaryKey));
        Expression<Func<object?>> closure = () => idValue;
        var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);
        var lambdaBody = Expression.Equal(leftExpression, rightExpression);
        return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParameter);
    }
}