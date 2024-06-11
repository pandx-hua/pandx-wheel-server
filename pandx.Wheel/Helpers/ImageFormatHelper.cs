using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace pandx.Wheel.Helpers;

public class ImageFormatHelper
{
    public static IImageFormat GetRawImageFormat(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        return Image.DetectFormat(bytes);
    }
}