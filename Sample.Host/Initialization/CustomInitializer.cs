using Microsoft.Extensions.Hosting;
using pandx.Wheel.Folders;
using pandx.Wheel.Initializers;

namespace Sample.Host.Initialization;

public class CustomInitializer : ICustomInitializer
{
    private readonly ICommonFolder _commonFolder;
    private readonly IHostEnvironment _hostEnvironment;

    public CustomInitializer(IHostEnvironment hostEnvironment, ICommonFolder commonFolder)
    {
        _hostEnvironment = hostEnvironment;
        _commonFolder = commonFolder;
    }

    public Task InitializeAsync()
    {
        _commonFolder.LogsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "Logs");
        _commonFolder.FilesFolder = Path.Combine(_hostEnvironment.ContentRootPath, "Files");
        _commonFolder.TempFolder = Path.Combine(_hostEnvironment.ContentRootPath, "Temp");
        _commonFolder.LargeFilesFolder = Path.Combine(_hostEnvironment.ContentRootPath, "LargeFiles");
        if (!Directory.Exists(_commonFolder.LogsFolder))
        {
            Directory.CreateDirectory(_commonFolder.LogsFolder);
        }

        if (!Directory.Exists(_commonFolder.FilesFolder))
        {
            Directory.CreateDirectory(_commonFolder.FilesFolder);
        }

        if (!Directory.Exists(_commonFolder.TempFolder))
        {
            Directory.CreateDirectory(_commonFolder.TempFolder);
        }

        if (!Directory.Exists(_commonFolder.LargeFilesFolder))
        {
            Directory.CreateDirectory(_commonFolder.LargeFilesFolder);
        }

        return Task.CompletedTask;
    }
}