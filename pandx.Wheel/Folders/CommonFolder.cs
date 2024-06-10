namespace pandx.Wheel.Folders;

public class CommonFolder : ICommonFolder
{
    public string LogsFolder { get; set; } = default!;
    public string FilesFolder { get; set; } = default!;
    public string TempFolder { get; set; } = default!;
    public string LargeFilesFolder { get; set; } = default!;
}