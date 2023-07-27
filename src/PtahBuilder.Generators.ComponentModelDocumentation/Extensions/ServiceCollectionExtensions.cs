using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PtahBuilder.BuildSystem;
using PtahBuilder.Generators.ComponentModelDocumentation.Abstractions;
using PtahBuilder.Generators.ComponentModelDocumentation.Services;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPtahComponentModelDocumentationServices(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentationProvider, ComponentModelDocumentationProvider>();
        services.AddSingleton<IObsoleteDocumentationService, ObsoleteDocumentationService>();

        return services;
    }

    public static IServiceCollection UseCustomDocumentationService<T>(this IServiceCollection services) where T : IDocumentationProvider
    {
        var descriptor = new ServiceDescriptor(typeof(IDocumentationProvider), typeof(T), ServiceLifetime.Singleton);

        services.Replace(descriptor);

        return services;
    }

    public static BuilderFactory AddComponentModelDocumentation(this BuilderFactory factory)
    {
        return factory.ConfigureServices(s => s.AddPtahComponentModelDocumentationServices());
    }

    public static BuilderFactory AddComponentModelDocumentation<T>(this BuilderFactory factory) where T : IDocumentationProvider
    {
        return factory.ConfigureServices(s =>
            {
                s.AddPtahComponentModelDocumentationServices()
                    .UseCustomDocumentationService<T>();
            });
    }
}