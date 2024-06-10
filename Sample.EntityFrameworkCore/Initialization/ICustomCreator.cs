using pandx.Wheel.DependencyInjection;

namespace Sample.EntityFrameworkCore.Initialization;

public interface ICustomCreator : ITransientDependency
{
    Task InitializeAsync();
}