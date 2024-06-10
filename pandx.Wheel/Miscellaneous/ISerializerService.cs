using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Miscellaneous;

public interface ISerializerService : ITransientDependency
{
    string Serialize<T>(T obj);

    string Serialize<T>(T obj, Type type);

    T? Deserialize<T>(string str);
}