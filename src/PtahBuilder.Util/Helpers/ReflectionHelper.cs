using PtahBuilder.Util.Extensions.Reflection;
using System.Reflection;

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
                        !string.IsNullOrWhiteSpace(a.FullName) && a.FullName.StartsWith(assemblyFilter));
    }

    public static IEnumerable<Type> GetAllLoadedTypes(string? assemblyFilter = null)
    {
        if (_allLoadedTypes == null)
        {
            _allLoadedTypes = new();

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
            return _allLoadedTypes.Where(x => !string.IsNullOrWhiteSpace(x.Assembly.FullName) && x.Assembly.FullName.StartsWith(assemblyFilter));
        }

        return _allLoadedTypes.ToArray();
    }

    public static IEnumerable<Type> GetLoadedTypesWithAttribute<T>(string? namespaceFilter = null) where T : Attribute
    {
        return GetAllLoadedTypes(namespaceFilter).Where(t => t.GetAttributeOfType<T>() != null);
    }

    public static Type GetLoadedTypeByFullName(string fullTypeName, string? assemblyFilter = null)
    {
        return GetAllLoadedTypes(assemblyFilter).First(t => t.FullName == fullTypeName);
    }

    public static IEnumerable<Type> GetLoadedTypesThatAreAssignableTo(Type type, bool instantiableOnly = true, string? assemblyFilter = null)
    {
        return GetAllLoadedTypes(assemblyFilter).Where(t => type.IsAssignableFrom(t) && (!instantiableOnly || (!t.IsAbstract && !t.IsInterface && !t.ContainsGenericParameters)));
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
}