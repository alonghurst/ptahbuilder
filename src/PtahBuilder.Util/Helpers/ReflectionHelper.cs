using PtahBuilder.Util.Extensions.Reflection;
using System.Reflection;

namespace PtahBuilder.Util.Helpers;

public static class ReflectionHelper
{
    private static readonly object LoadedTypesGate = new();
    private static List<Type>? _allLoadedTypes;
    private static int _cachedAssemblyCount;

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
        var types = SnapshotLoadedTypes();

        if (assemblyFilter != null)
        {
            return types.Where(x => !string.IsNullOrWhiteSpace(x.Assembly.FullName) && x.Assembly.FullName.StartsWith(assemblyFilter));
        }

        return types.ToArray();
    }

    private static List<Type> SnapshotLoadedTypes()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var snapshot = Volatile.Read(ref _allLoadedTypes);
        if (snapshot != null && Volatile.Read(ref _cachedAssemblyCount) == assemblies.Length)
        {
            return snapshot;
        }

        lock (LoadedTypesGate)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (_allLoadedTypes != null && _cachedAssemblyCount == assemblies.Length)
            {
                return _allLoadedTypes;
            }

            var loaded = new List<Type>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    loaded.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException exception)
                {
                    foreach (var type in exception.Types)
                    {
                        if (type != null)
                        {
                            loaded.Add(type);
                        }
                    }
                }
                catch
                {
                    // Dynamic assemblies can refuse GetTypes.
                }
            }

            _cachedAssemblyCount = assemblies.Length;
            Volatile.Write(ref _allLoadedTypes, loaded);
            return loaded;
        }
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