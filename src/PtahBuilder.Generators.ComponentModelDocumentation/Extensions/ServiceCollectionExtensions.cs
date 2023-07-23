using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Extensions
{
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
}
