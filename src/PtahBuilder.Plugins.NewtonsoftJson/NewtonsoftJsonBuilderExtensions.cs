using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Plugins.NewtonsoftJson.Config.Internal;
using PtahBuilder.Plugins.NewtonsoftJson.Services;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.Plugins.NewtonsoftJson;

public static class NewtonsoftJsonBuilderExtensions
{
    public static BuilderFactory UseNewtonsoftJson(this BuilderFactory builderFactory, Assembly jsonConvertersAssembly)
    {
        var converters = GetConvertersFromAssembly(jsonConvertersAssembly);

        return builderFactory.UseNewtonsoftJson(converters.ToArray());
    }

    public static BuilderFactory UseNewtonsoftJson(this BuilderFactory builderFactory, Assembly jsonConvertersAssembly, JsonSerializerSettings settings)
    {
        var converters = GetConvertersFromAssembly(jsonConvertersAssembly);

        return builderFactory.UseNewtonsoftJson(settings, converters.ToArray());
    }

    public static BuilderFactory UseNewtonsoftJson(this BuilderFactory builderFactory, params JsonConverter[] converters)
    {
        var settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        return builderFactory.UseNewtonsoftJson(settings, converters);
    }

    public static BuilderFactory UseNewtonsoftJson(this BuilderFactory builderFactory, JsonSerializerSettings settings, params JsonConverter[] converters)
    {
        builderFactory.ConfigureServices(services =>
        {
            services.AddSingleton(new NewtonsoftJsonConverterConfig(converters));

            services.AddSingleton(settings);

            var descriptor = new ServiceDescriptor(typeof(IJsonService), typeof(NewtonsoftJsonService), ServiceLifetime.Singleton);

            services.Replace(descriptor);
        });

        return builderFactory;
    }

    public static BuilderFactory ReplaceNewtonsoftJsonSettings(this BuilderFactory builderFactory, JsonSerializerSettings settings)
    {
        builderFactory.ConfigureServices(services =>
        {
            var descriptor = new ServiceDescriptor(typeof(JsonSerializerSettings), settings);

            services.Replace(descriptor);
        });

        return builderFactory;
    }

    private static List<JsonConverter> GetConvertersFromAssembly(Assembly jsonConvertersAssembly)
    {
        var types = ReflectionHelper.GetLoadedTypesThatAreAssignableTo(typeof(JsonConverter), assemblyFilter: jsonConvertersAssembly.FullName!);

        var converters = new List<JsonConverter>();

        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is JsonConverter instance)
            {
                converters.Add(instance);
            }
        }

        return converters;
    }

}