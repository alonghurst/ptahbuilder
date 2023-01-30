using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace PtahBuilder.Util.Helpers;

public static class ReflectionHelper
{
    private static List<Type>? _allLoadedTypes = null;

    public static IReadOnlyCollection<T> GetEnumValues<T>()
    {
        return (Enum.GetValues(typeof(T)) as T[])!;
    }

    public static IEnumerable<Assembly> GetLoadedAssemblies(string? assemblyFilter = null)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => string.IsNullOrWhiteSpace(assemblyFilter) ||
                        (!string.IsNullOrWhiteSpace(a.FullName) && a.FullName.StartsWith(assemblyFilter)));
    }

    public static IEnumerable<Type> GetAllLoadedTypes(string? assemblyFilter = null)
    {
        if (_allLoadedTypes == null)
        {
            _allLoadedTypes = new List<Type>();

            var assemblies = GetLoadedAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        _allLoadedTypes.Add(type);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        if (assemblyFilter != null)
        {
            return _allLoadedTypes.Where(x => (!string.IsNullOrWhiteSpace(x.Assembly.FullName) && x.Assembly.FullName.StartsWith(assemblyFilter)));
        }

        return _allLoadedTypes.ToArray();
    }

    public static Type GetLoadedTypeByFullName(string fullTypeName, string? assemblyFilter = null)
    {
        return GetAllLoadedTypes(assemblyFilter).First(t => t.FullName == fullTypeName);
    }

    public static IEnumerable<Type> GetLoadedTypesThatAreAssignableTo(Type type, bool instantiableOnly = true, string? assemblyFilter = null)
    {
        return GetAllLoadedTypes(assemblyFilter).Where(t => type.IsAssignableFrom(t) && (!instantiableOnly || !t.IsAbstract && !t.IsInterface));
    }

    public static IEnumerable<Type> GetLoadedTypesThatImplementInterfaceWithGenericArgumentsOfType(Type interfaceType, string? namespaceFilter, params Type[] genericTypes)
    {
        if (!interfaceType.IsInterface || !interfaceType.IsGenericType)
        {
            throw new InvalidOperationException($"Type \"{interfaceType.Name}\" is not an interface or is not a generic type");
        }

        var constructedType = interfaceType.MakeGenericType(genericTypes);

        return GetLoadedTypesThatAreAssignableTo(constructedType, assemblyFilter: namespaceFilter);
    }

    public static bool IsEnumerable(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
               || type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static Type GetTypeOrElementType(this Type type)
    {
        var interfaceType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ? type : type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (interfaceType != null && type != typeof(string))
        {
            return interfaceType.GetGenericArguments()[0];
        }

        return type;
    }

    public static bool ImplementInterface(this Type type, Type @interface)
    {
        return type.GetInterfaces().Any(i => i == @interface || (i.IsGenericType && i.GetGenericTypeDefinition() == @interface));
    }

    public static Type GetTypeOrElementType(this object o)
    {
        var type = o.GetType();
        return type.GetTypeOrElementType();
    }

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

    public static IEnumerable<Type> GetLoadedTypesWithAttribute<T>(string? namespaceFilter = null) where T : Attribute
    {
        return GetAllLoadedTypes(namespaceFilter).Where(t => GetAttributeOfType<T>(t) != null);
    }

    public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type) where T : Attribute
    {
        return type.GetProperties().Where(p => p.GetCustomAttribute<T>(true) != null);
    }

    public static IEnumerable<PropertyInfo> GetPropertiesOfType<T>(this Type type)
    {
        return type.GetProperties().Where(p => typeof(T).IsAssignableFrom(p.PropertyType));
    }

#nullable disable
    public static async Task<object> InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
    {
        var task = (Task)method.Invoke(obj, parameters);
        await task.ConfigureAwait(false);
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty?.GetValue(task);
    }
#nullable enable

    public static MethodInfo[] GetMethodsWithAttribute<T>(this Type type)
    {
        return type.GetMethods()
            .Where(p => p.GetCustomAttributes(true).Any(a => a is T))
            .ToArray();
    }

    public static T[] InstantiateTypesThatAreAssignableTo<T>(this IServiceProvider services)
    {
        var instances = GetLoadedTypesThatAreAssignableTo(typeof(T))
            .Select(x => ActivatorUtilities.CreateInstance(services, x))
            .OfType<T>()
            .ToArray();

        return instances;
    }

    public static Type? GetBaseGenericType(Type type)
    {
        if (type.BaseType == null || type.BaseType == typeof(object))
        {
            return null;
        }

        if (type.BaseType.GenericTypeArguments.Any())
        {
            return type.BaseType.GenericTypeArguments[0];
        }

        return GetBaseGenericType(type.BaseType);
    }

}