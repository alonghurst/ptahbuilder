using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Config.Internal;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.Util.Extensions;
using PtahBuilder.Util.Helpers;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem;

// Root class that builds it all
public class BuilderFactory
{
    private readonly Dictionary<Type, Func<object, object?>> _customValueParsers = new();

    private readonly List<JsonConverter> _jsonConverters = new();

    private List<Action<IServiceCollection>> _configureServices = new();
    private readonly List<Action<ExecutionConfig, DateTime>> _afterSuccessfulRun = new();
    private Action<ExecutionConfig>? _configureExecutionConfig;

    private readonly FilesConfig _filesConfig = new();

    public BuilderFactory ConfigureFiles(Action<FilesConfig> configureFiles)
    {
        configureFiles(_filesConfig);

        return this;
    }

    public BuilderFactory AddJsonConverterTypes(Assembly assembly)
    {
        var types = ReflectionHelper.GetLoadedTypesThatAreAssignableTo(typeof(JsonConverter), assemblyFilter: assembly.FullName!);

        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is JsonConverter instance)
            {
                AddJsonConverters(instance);
            }
        }

        return this;
    }

    public BuilderFactory AddJsonConverters(params JsonConverter[] converters)
    {
        foreach (var jsonConverter in converters)
        {
            _jsonConverters.Add(jsonConverter);
        }

        return this;
    }

    public BuilderFactory AddCustomValueParser(Type type, Func<object, object?> customValueParser)
    {
        _customValueParsers.Add(type, customValueParser);

        return this;
    }

    public BuilderFactory ConfigureServices(Action<IServiceCollection> configureServices)
    {
        _configureServices.Add(configureServices);

        return this;
    }

    public BuilderFactory ConfigureExecution(Action<ExecutionConfig> configureExecution)
    {
        _configureExecutionConfig = configureExecution;

        return this;
    }

    /// <summary>
    /// Invoked after a successful run. <paramref name="after"/> receives the UTC time captured
    /// at the start of <see cref="Run"/>, before output files are written.
    /// </summary>
    public BuilderFactory AfterSuccessfulRun(Action<ExecutionConfig, DateTime> after)
    {
        _afterSuccessfulRun.Add(after);

        return this;
    }

    public async Task Run()
    {
        var writtenAfterUtc = DateTime.UtcNow;
        var services = BuildServices();
        BuilderContext? context = null;

        try
        {
            var config = BuildExecutionConfig();
            context = new BuilderContext(services, config);

            if (config.PreExecution != null)
            {
                config.PreExecution(context);
            }

            await context.Run();

            foreach (var after in _afterSuccessfulRun)
            {
                after(config, writtenAfterUtc);
            }
        }
        finally
        {
            context?.Dispose();

            foreach (var descriptor in services)
            {
                if (descriptor.ImplementationInstance is RootLogger logger)
                {
                    logger.Dispose();
                }
            }
        }
    }

    private ExecutionConfig BuildExecutionConfig()
    {
        var executionConfig = new ExecutionConfig(_filesConfig);

        _configureExecutionConfig?.Invoke(executionConfig);

        if (!Directory.Exists(executionConfig.Files.DataDirectory))
        {
            Directory.CreateDirectory(executionConfig.Files.DataDirectory);
        }

        return executionConfig;
    }

    private IServiceCollection BuildServices()
    {
        var services = new ServiceCollection()
                .AddPtahUtilServices()
                .AddPtahBuildSystemServices()
                .AddSingleton(new CustomValueParserConfig(_customValueParsers))
                .AddSingleton(new JsonConverterConfig(_jsonConverters))
                .AddSingleton<IFilesConfig>(_filesConfig);

        foreach (var configureService in _configureServices)
        {
            configureService.Invoke(services);
        }

        return services;
    }
}