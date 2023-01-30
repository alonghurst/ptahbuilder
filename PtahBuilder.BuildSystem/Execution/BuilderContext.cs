using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Execution;

public class BuilderContext : IDisposable
{
    private readonly IServiceCollection _services;
    private readonly ExecutionConfig _config;
    private readonly ILogger _logger;

    private ServiceProvider _currentServiceProvider;

    public BuilderContext(IServiceCollection services, ExecutionConfig config)
    {
        _services = services;
        _config = config;

        _currentServiceProvider = _services.BuildServiceProvider();
        _logger = _currentServiceProvider.GetRequiredService<ILogger>();
    }

    public async Task Run()
    {
        OutputConfiguration();

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _currentServiceProvider.Dispose();
    }

    private void OutputConfiguration()
    {
        var stages = Enum.GetValues<Stage>();

        var json = _currentServiceProvider.GetRequiredService<IJsonService>();
        var files = _currentServiceProvider.GetRequiredService<IFilesConfig>();

        _logger.Info($"Files:");
        _logger.Info(json.Serialize(files));

        foreach (var entityPipeline in _config.EntityPipelines)
        {
            _logger.Info($"{entityPipeline.Value.Name}: {entityPipeline.Key.FullName ?? entityPipeline.Key.Name}");

            foreach (var stage in stages)
            {
                if (entityPipeline.Value.Stages.TryGetValue(stage, out var steps) && steps.Any())
                {
                    foreach (var step in steps)
                    {
                        _logger.Info($"\t{stage}: {step.StageType.FullName ?? step.StageType.Name}");
                    }
                }
            }
        }
    }
}