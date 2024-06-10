namespace pandx.Wheel.Extensions;

public static class StreamExtensions
{
    public static byte[] GetAllBytes(this Stream stream)
    {
        using var destination = new MemoryStream();
        stream.CopyTo(destination);
        return destination.ToArray();
    }

    public static async Task<byte[]> GetAllBytesAsync(this Stream stream)
    {
        using var destination = new MemoryStream();
        await stream.CopyToAsync(destination);
        return destination.ToArray();
    }
}