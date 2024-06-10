namespace pandx.Wheel.DependencyInjection;

public static class ServiceLocator
{
    public static IServiceProvider Instance { get; set; } = default!;
}