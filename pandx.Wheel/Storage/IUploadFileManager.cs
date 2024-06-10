using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Storage;

public interface IUploadFileManager : ITransientDependency
{
    Task<List<UploadFile>> UploadLargeFilesToDiskAsync();
    Task<UploadFile> UploadLargeFileToDiskAsync();
    Task<List<UploadFile>> UploadFilesToDiskAsync();
    Task<UploadFile> UploadFileToDiskAsync();
    Task<CachedFile> UploadFileToCacheAsync();
}