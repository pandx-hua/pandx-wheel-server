using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Miscellaneous;

public interface IClientInfoProvider : ITransientDependency
{
    string? BrowserInfo { get; }
    string? ClientIpAddress { get; }
    string? ComputerName { get; }
}