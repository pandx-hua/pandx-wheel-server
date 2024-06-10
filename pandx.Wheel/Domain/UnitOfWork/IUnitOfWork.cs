namespace pandx.Wheel.Domain.UnitOfWork;

//ScopedDependency,保证在一次请求中使用的是同一个DbContext
public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}