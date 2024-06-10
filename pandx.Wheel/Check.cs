namespace pandx.Wheel;

public static class Check
{
    public static T NotNull<T>(T value, string parameterName)
    {
        _ = value ?? throw new ArgumentNullException(parameterName);

        return value;
    }
}