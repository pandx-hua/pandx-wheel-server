using System.Text;

namespace pandx.Wheel.Helpers;

public static class FileHelper
{
    public static IEnumerable<string> ReadLines(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000,
            FileOptions.SequentialScan);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        while (sr.ReadLine() is { } line)
        {
            yield return line;
        }
    }
}