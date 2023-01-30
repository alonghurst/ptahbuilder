using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Config.Internal;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services;
using PtahBuilder.Util.Extensions;

namespace PtahBuilder.BuildSystem;

public class BuilderFactory
{
    private readonly Dictionary<Type, Func<object, object>> _customValueParsers = new ();

    private Action<IServiceCollection>? _configureServices;
    private Action<ExecutionConfig>? _configureExecutionConfig;

    private readonly FilesConfig _filesConfig = new();

    public BuilderFactory ConfigureFiles(Action<FilesConfig> configureFiles)
    {
        configureFiles(_filesConfig);

        return this;
    }

    public BuilderFactory AddCustomValueParser(Type type, Func<object, object> customValueParser)
    {
        _customValueParsers.Add(type, customValueParser);

        return this;
    }

    public BuilderFactory ConfigureServices(Action<IServiceCollection> configureServices)
    {
        _configureServices = configureServices;

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
            await context.Run();
        }
        finally
        {
            context.Dispose();
        }
    }

    private ExecutionConfig BuildExecutionConfig()
    {
        var executionConfig = new ExecutionConfig();

        _configureExecutionConfig?.Invoke(executionConfig);

        return executionConfig;
    }

    private IServiceCollection BuildServices()
    {
        var services = new ServiceCollection()
                .AddPtahUtilServices()
                .AddPtahBuildSystemServices()
                .AddSingleton<CustomValueParserConfig>(new CustomValueParserConfig(_customValueParsers))
                .AddSingleton<IFilesConfig>(_filesConfig);
        
        _configureServices?.Invoke(services);

        return services;
    }
}