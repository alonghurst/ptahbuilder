using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Extensions;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Execution;

public class BuilderContext 
{
    private readonly IServiceCollection _services;
    private readonly ExecutionConfig _config;
    private readonly ILogger _logger;
    private readonly IDiagnostics _diagnostics;

    public BuilderContext(IServiceCollection services, ExecutionConfig config)
    {
        _services = services;
        _config = config;

        using var serviceProvider = _services.BuildServiceProvider();
        _logger = serviceProvider.GetRequiredService<ILogger>();
        _diagnostics = serviceProvider.GetRequiredService<IDiagnostics>();
    }

    public async Task Run()
    {
        await OutputConfiguration();

        var pipelines = BuildPipelines().ToArray();

        foreach (var (type, pipeline) in pipelines)
        {
            var providerType = typeof(IEntityProvider<>).MakeGenericType(type);

            if (pipeline.GetType().IsAssignableTo(providerType))
            {
                _services.AddSingleton(providerType, pipeline);
            }
            else
            {
                _logger.Warning($"Unable to assign {pipeline.GetType().GetTypeName()} to {providerType.GetTypeName()}");
            }
        }

        await using var serviceProvider = _services.BuildServiceProvider();

        var stages = Enum.GetValues<Stage>();

        foreach (var stage in stages)
        {
            _logger.Info($"Executing stage: {stage}");

            foreach (var (_, pipeline) in pipelines)
            {
                await pipeline.ProcessStepsInStage(stage, serviceProvider);
            }
        }
    }

    private IEnumerable<(Type type, IPipelineContext pipeline)> BuildPipelines()
    {
        foreach (var config in _config.EntityPipelines)
        {
            var pipelineType = typeof(PipelineContext<>).MakeGenericType(config.Key);

            var pipeline = Activator.CreateInstance(pipelineType, config, _logger) as IPipelineContext;

            if (pipeline == null)
            {
                throw new InvalidOperationException($"Unable to instantiate pipeline for {config.Key.Name}");
            }

            yield return (config.Key, pipeline);
        }
    }
    
    private async Task OutputConfiguration()
    {
        await using var serviceProvider = _services.BuildServiceProvider();

        var stages = Enum.GetValues<Stage>();

        var json = serviceProvider.GetRequiredService<IJsonService>();
        var files = serviceProvider.GetRequiredService<IFilesConfig>();

        _logger.Info($"Files:");
        _logger.Info(json.Serialize(files));

        foreach (var entityPipeline in _config.EntityPipelines)
        {
            _logger.Info($"{entityPipeline.Value.Name}: {entityPipeline.Key.GetTypeName()}");

            foreach (var stage in stages)
            {
                if (entityPipeline.Value.Stages.TryGetValue(stage, out var steps) && steps.Any())
                {
                    foreach (var step in steps)
                    {
                        _logger.Info($"\t{stage}: {step.StepType.GetTypeName()}");
                    }
                }
            }
        }
    }
}