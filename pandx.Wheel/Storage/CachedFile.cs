namespace pandx.Wheel.Storage;

public class CachedFile
{
    public CachedFile()
    {
    }

    public CachedFile(string name, string type)
    {
        Name = name;
        Type = type;
        Token = Guid.NewGuid().ToString();
    }

    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Token { get; set; } = default!;
}