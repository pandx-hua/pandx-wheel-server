namespace pandx.Wheel.Models;

[Serializable]
public class PackagedResponse
{
    public bool Success { get; set; }
    public object Result { get; set; } = default!;
    public bool Pandx { get; } = true;
}