using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PtahBuilder.Util.Extensions.Reflection;

public static class ReflectionExtensions
{
    

    public static Type? GetFirstGenericTypeOfNonGenericInterface(Type type, Type baseInterfaceType)
    {
        var firstInterface = type.GetInterfaces().FirstOrDefault(f => baseInterfaceType.IsAssignableFrom(f) && f.IsGenericType);

        return firstInterface?.GetGenericArguments()[0];
    }

    public static T? GetAttributeOfType<T>(this Type type) where T : Attribute
    {
        return type.GetCustomAttribute<T>(true);
    }

    public static T? GetAttributeOfType<T>(this PropertyInfo property) where T : Attribute
    {
        return property.GetCustomAttribute<T>(true);
    }
    public static bool HasAttributeOfType<T>(this PropertyInfo property) where T : Attribute
    {
        return property.GetCustomAttribute<T>(true) != null;
    }

    public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type) where T : Attribute
    {
        return type.GetProperties().Where(p => p.GetCustomAttribute<T>(true) != null);
    }

    public static IEnumerable<PropertyInfo> GetPropertiesOfType<T>(this Type type)
    {
        return type.GetProperties().Where(p => typeof(T).IsAssignableFrom(p.PropertyType));
    }

    public static MethodInfo[] GetMethodsWithAttribute<T>(this Type type)
    {
        return type.GetMethods()
            .Where(p => p.GetCustomAttributes(true).Any(a => a is T))
            .ToArray();
    }
}