using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.Util.Extensions;

namespace PtahBuilder.BuildSystem;

public class BuilderFactory
{
    private Action<IServiceCollection>? _configureServices;
    private Action<ExecutionConfig>? _configureExecutionConfig;

    private readonly FilesConfig _filesConfig = new();

    public BuilderFactory ConfigureFiles(Action<FilesConfig> configureFiles)
    {
        configureFiles(_filesConfig);

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

    public Task Run()
    {
        var services = BuildServices();
        var config = BuildExecutionConfig();

        var context = new BuilderContext(services, config);

        return context.Run();
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
                .AddSingleton<IFilesConfig, FilesConfig>();

        _configureServices?.Invoke(services);

        return services;
    }
}