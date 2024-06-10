namespace pandx.Wheel.Extensions;

public static class StringExtensions
{
    public static string RemovePostFix(this string str, params string[] postFixes)
    {
        _ = str ?? throw new ArgumentException(nameof(str));
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        if (postFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var postFix in postFixes)
        {
            if (str.EndsWith(postFix))
            {
                return str.Left(str.Length - postFix.Length);
            }
        }

        return str;
    }

    public static string Left(this string str, int length)
    {
        _ = str ?? throw new ArgumentException(nameof(str));
        if (str.Length < length)
        {
            throw new ArgumentException($"参数 {nameof(length)} 不能大于字符串长度");
        }

        return str.Substring(0, length);
    }

    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string RemoveAsync(this string str)
    {
        if (str.IndexOf("Async", StringComparison.Ordinal) < 0)
        {
            throw new ArgumentException(nameof(str));
        }

        return str.Substring(0, str.Length - 5);
    }
}