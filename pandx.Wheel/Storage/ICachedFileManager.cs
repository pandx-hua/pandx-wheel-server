using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Storage;

public interface ICachedFileManager : ITransientDependency
{
    Task SetFileAsync(string token, string name, byte[] content);
    Task<byte[]?> GetFileAsync(string token, string name);
    Task SaveFileAsync(string token, string name);
}