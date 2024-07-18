using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Config.Internal;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.Util.Extensions;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.BuildSystem;

public class BuilderFactory
{
    private readonly Dictionary<Type, Func<object, object>> _customValueParsers = new();

    private readonly List<JsonConverter> _jsonConverters = new();

    private List<Action<IServiceCollection>> _configureServices = new();
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

    public BuilderFactory AddCustomValueParser(Type type, Func<object, object> customValueParser)
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

    public async Task Run()
    {
        var services = BuildServices();
        var config = BuildExecutionConfig();

        var context = new BuilderContext(services, config);

        try
        {
            if (config.PreExecution != null)
            {
                config.PreExecution(context);
            }

            await context.Run();
        }
        finally
        {
            context.Dispose();
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