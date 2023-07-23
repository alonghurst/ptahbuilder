using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPtahComponentModelDocumentationServices(this IServiceCollection services)
    {
        return services;
    }

    public static BuilderFactory AddComponentModelDocumentation(this BuilderFactory factory)
    {
        return factory.ConfigureServices(s => s.AddPtahComponentModelDocumentationServices());
    }
}