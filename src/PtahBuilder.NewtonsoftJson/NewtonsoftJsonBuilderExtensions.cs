using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.NewtonsoftJson.Config.Internal;
using PtahBuilder.NewtonsoftJson.Services;

namespace PtahBuilder.NewtonsoftJson
{
    public static class NewtonsoftJsonBuilderExtensions
    {
        public static BuilderFactory UseNewtonsoftJson(this BuilderFactory builderFactory, params JsonConverter[] converters)
        {
            builderFactory.ConfigureServices(services =>
            {
                services.AddSingleton(new NewtonsoftJsonConverterConfig(converters));

                var descriptor = new ServiceDescriptor(typeof(IJsonService), typeof(NewtonsoftJsonService), ServiceLifetime.Singleton);

                services.Replace(descriptor);
            });

            return builderFactory;
        }
    }
}