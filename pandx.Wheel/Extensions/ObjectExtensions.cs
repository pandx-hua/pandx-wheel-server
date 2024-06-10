using System.ComponentModel;
using System.Globalization;

namespace pandx.Wheel.Extensions;

public static class ObjectExtensions
{
    public static T As<T>(this object obj) where T : class
    {
        return (T)obj;
    }

    public static T To<T>(this object obj) where T : struct
    {
        if (typeof(T) == typeof(Guid) || typeof(T) == typeof(TimeSpan))
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
        }

        if (!typeof(T).IsEnum)
        {
            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        return Enum.IsDefined(typeof(T), obj)
            ? (T)Enum.Parse(typeof(T), obj.ToString() ?? string.Empty)
            : throw new ArgumentException($"枚举类型 {obj} 未定义");
    }

    public static bool IsIn<T>(this T item, params T[] list)
    {
        return list.Contains(item);
    }
}