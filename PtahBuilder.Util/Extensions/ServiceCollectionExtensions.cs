using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.Util.Helpers;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.Util.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPtahUtilServices(this IServiceCollection services, bool logToFile = true)
    {
        var @namespace = (typeof(ServiceCollectionExtensions).Namespace ?? string.Empty).Replace(".Extensions", string.Empty);
        services.AddInterfaceImplementations(@namespace, ScopeType.Singleton);

        var loggers = services.Where(x => x.ServiceType == typeof(ILogger)).ToArray();

        foreach (var logger in loggers)
        {
            services.Remove(logger);
        }

        if (logToFile)
        {
            var rootLogger = new RootLogger();

            services.AddSingleton<ILogger>(rootLogger);
        }
        else
        {
            services.AddSingleton<ILogger, ConsoleLogger>();
        }

        return services;
    }
}