using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Initializers;

public interface ICustomInitializer : ITransientDependency
{
    Task InitializeAsync();
}