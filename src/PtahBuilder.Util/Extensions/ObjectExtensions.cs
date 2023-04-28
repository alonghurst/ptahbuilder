using System.Reflection;

namespace PtahBuilder.Util.Extensions;

public static class ObjectExtensions
{
    public static T ShallowClone<T>(this T obj) where T : class
    {
        var clone = typeof(T)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(obj, null)!;

        return (T)clone;
    }
}