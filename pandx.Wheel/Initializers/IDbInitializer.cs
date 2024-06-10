using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Initializers;

public interface IDbInitializer : ITransientDependency
{
    Task InitializeAsync();
}