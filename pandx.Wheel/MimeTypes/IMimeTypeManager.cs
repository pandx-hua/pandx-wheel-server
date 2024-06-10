using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.MimeTypes;

public interface IMimeTypeManager : ITransientDependency
{
    bool TryGetMimeType(string str, out string? mimeType);
    string? GetMimeType(string str, bool throwErrorIfNotFound = true);
    bool TryGetExtension(string mimeType, out string? extension);
    string? GetExtension(string mimeType, bool throwErrorIfNotFound = true);
    void AddMimeType(string mimeType, string extension);
    void RemoveMimeType(string mimeType);
    void AddExtension(string extension, string mimeType);
    void RemoveExtension(string extension);
}