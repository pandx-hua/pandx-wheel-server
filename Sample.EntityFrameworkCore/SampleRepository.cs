using pandx.Wheel.Domain.Entities;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Persistence.EntityFrameworkCore;

namespace Sample.EntityFrameworkCore;

public class SampleRepository<TEntity, TPrimaryKey> : EfCoreRepository<SampleDbContext, TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
{
    public SampleRepository(SampleDbContext context) : base(context)
    {
    }
}

//明确指出实现IRepository<TEntity>接口，否则会出现实现类型无法转换为服务类型的错误，参考ABP的解决方法
public class SampleRepository<TEntity> : SampleRepository<TEntity, int>, IRepository<TEntity>
    where TEntity : class, IEntity<int>
{
    public SampleRepository(SampleDbContext context) : base(context)
    {
    }
}