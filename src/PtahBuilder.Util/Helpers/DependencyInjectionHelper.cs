using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Util.Helpers;

public enum ScopeType
{
    Scoped,
    Transient,
    Singleton
}

public static class DependencyInjectionHelper
{
    public static IServiceCollection AddInterfaceImplementations(this IServiceCollection collection, string namespacePath, ScopeType scope, string? namespaceFilter = null)
    {
        var interfaces = ReflectionExtensions.GetAllLoadedTypes(namespaceFilter)
            .Where(c => c.IsInterface &&
                        !string.IsNullOrWhiteSpace(c.Namespace) &&
                        c.Namespace.StartsWith(namespacePath));

        foreach (var @interface in interfaces)
        {
            collection.AddTypesThatImplement(@interface, scope, namespaceFilter);
        }

        return collection;
    }

    public static IServiceCollection AddTypesThatImplement(this IServiceCollection collection, Type serviceType, ScopeType scope, string? namespaceFilter = null)
    {
        var types = ReflectionExtensions.GetLoadedTypesThatAreAssignableTo(serviceType, assemblyFilter: namespaceFilter).Where(x => x.IsClass);

        foreach (var implementationType in types)
        {
            if (scope == ScopeType.Scoped)
            {
                collection.AddScoped(serviceType, implementationType);
            }
            else if (scope == ScopeType.Transient)
            {
                collection.AddTransient(serviceType, implementationType);
            }
            else if (scope == ScopeType.Singleton)
            {
                collection.AddSingleton(serviceType, implementationType);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        return collection;
    }

    public static IServiceCollection AddScopedTypesThatHaveAttribute<T>(this IServiceCollection collection, string? namespaceFilter = null) where T : Attribute
    {
        var types = ReflectionExtensions.GetLoadedTypesWithAttribute<T>(namespaceFilter);

        foreach (var t in types.Where(t => !t.IsAbstract))
        {
            collection.AddScoped(t);
        }

        return collection;
    }

    public static IServiceCollection AddScopedTypesThatImplement<T>(this IServiceCollection collection, string? namespaceFilter = null)
    {
        if (!typeof(T).IsInterface || typeof(T).IsGenericType)
        {
            throw new InvalidOperationException($"{typeof(T).FullName} must be a non-generic interface");
        }

        var types = ReflectionExtensions.GetLoadedTypesThatAreAssignableTo(typeof(T), assemblyFilter: namespaceFilter);

        foreach (var t in types)
        {
            collection.AddScoped(t);
        }

        return collection;
    }
}