namespace pandx.Wheel.Storage;

public class UploadFile
{
    public string OriginalName { get; set; } = default!;
    public string SavedName { get; set; } = default!;
    public double FileSize { get; set; }
    public string ContentType { get; set; } = default!;
    public bool IsSuccess { get; set; }
}