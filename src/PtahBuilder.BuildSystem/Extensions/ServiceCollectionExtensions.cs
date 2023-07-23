using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.BuildSystem.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPtahBuildSystemServices(this IServiceCollection services)
    {
        var @namespace = (typeof(ServiceCollectionExtensions).Namespace ?? string.Empty).Replace(".Extensions", ".Services");
        services.AddInterfaceImplementations(@namespace, ScopeType.Singleton);
        
        return services;
    }
}