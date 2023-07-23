using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.Util.Extensions.Reflection;

public static class ServiceCollectionExtensions
{
    public static T[] InstantiateTypesThatAreAssignableTo<T>(this IServiceProvider services)
    {
        var instances = ReflectionHelper.GetLoadedTypesThatAreAssignableTo(typeof(T))
            .Select(x => ActivatorUtilities.CreateInstance(services, x))
            .OfType<T>()
            .ToArray();

        return instances;
    }

}