using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Storage;

public interface IBinaryObjectManager:ITransientDependency
{
    Task<BinaryObject?> GetAsync(Guid id);
    Task SaveAsync(BinaryObject file);
    Task DeleteAsync(Guid id);
}