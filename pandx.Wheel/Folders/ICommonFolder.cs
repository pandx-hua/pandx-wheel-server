using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Folders;

public interface ICommonFolder : ISingletonDependency
{
    string LogsFolder { get; set; }
    string FilesFolder { get; set; }
    string TempFolder { get; set; }
    string LargeFilesFolder { get; set; }
}