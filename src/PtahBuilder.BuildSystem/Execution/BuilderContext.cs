using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Extensions;
using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Execution;

public class BuilderContext : IDisposable
{
    private readonly IServiceCollection _services;
    private readonly ExecutionConfig _config;
    private readonly ILogger _logger;
    private readonly IDiagnostics _diagnostics;
    private readonly ServiceProvider _serviceProvider;

    public BuilderContext(IServiceCollection services, ExecutionConfig config)
    {
        _services = services;
        _config = config;

        _serviceProvider = _services.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
        _diagnostics = _serviceProvider.GetRequiredService<IDiagnostics>();

        if (config.DeleteOutputDirectory)
        {
            var output = _serviceProvider.GetRequiredService<IFilesConfig>().OutputDirectory;

            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
        }
    }

    public async Task Run()
    {
        OutputConfiguration();

        var pipelines = BuildPipelines().ToArray();

        var stages = Enum.GetValues<Stage>();

        var phasedPipelines = pipelines
            .GroupBy(x => x.pipeline.Phase)
            .OrderBy(x => x.Key)
            .ToArray();

        foreach (var phaseGroup in phasedPipelines)
        {
            foreach (var (type, pipeline) in phaseGroup)
            {
                var providerType = typeof(IEntityProvider<>).MakeGenericType(type);

                if (pipeline.GetType().IsAssignableTo(providerType))
                {
                    var descriptor = new ServiceDescriptor(providerType, pipeline);

                    _services.Replace(descriptor);
                    
                    _logger.Info($"Added {pipeline.GetType().GetTypeName()} as {providerType.GetTypeName()}");
                }
                else
                {
                    _logger.Warning($"Unable to assign {pipeline.GetType().GetTypeName()} to {providerType.GetTypeName()}");
                }
            }

            await using var serviceProvider = _services.BuildServiceProvider();

            _logger.Info($"Executing phase: {phaseGroup.Key}".Colour(ConsoleColor.Magenta));

            foreach (var stage in stages)
            {
                _logger.Info($"Executing stage: {stage}".Colour(ConsoleColor.Blue));

                foreach (var (_, pipeline) in phaseGroup)
                {
                    await pipeline.ProcessStepsInStage(stage, serviceProvider);
                }
            }
        }

        var validationFilePath = Path.Combine(_config.Files.DataDirectory, "validation.txt");
        var validationErrors = pipelines.SelectMany(x => x.pipeline.ValidationErrors()).ToArray();

        if (validationErrors.Any())
        {
            var sb = new StringBuilder();

            void OutputError(string message)
            {
                _logger.Warning(message);
                sb!.AppendLine(message);
            }

            var grouped = validationErrors.GroupBy(x => x.type);

            foreach (var group in grouped)
            {
                OutputError($"{group.Key.Name} Validation Errors");
                foreach (var entity in group)
                {
                    OutputError($"  {entity.id}");
                    foreach (var error in entity.errors)
                    {
                        OutputError($"    {error.Source}: {error.Error}");
                    }
                }
            }

            _logger.Warning("Execution completed with validation errors.");

            await File.WriteAllTextAsync(validationFilePath, sb.ToString());
        }
        else
        {
            File.Delete(validationFilePath);
            _logger.Success("Execution completed with no validation errors.");
        }
    }

    private IEnumerable<(Type type, IPipelineContext pipeline)> BuildPipelines()
    {
        foreach (var config in _config.EntityPipelines)
        {
            var type = config.GetType();

            if (type.GenericTypeArguments.Length > 0)
            {
                var entityType = type.GetGenericArguments()[0];

                var pipelineType = typeof(PipelineContext<>).MakeGenericType(entityType);

                var pipeline = Activator.CreateInstance(pipelineType, config, _logger, _diagnostics) as IPipelineContext;

                if (pipeline == null)
                {
                    throw new InvalidOperationException($"Unable to instantiate pipeline for {entityType.GetTypeName()}");
                }

                yield return (entityType, pipeline);
            }
        }
    }

    private void OutputConfiguration()
    {
        var stages = Enum.GetValues<Stage>();

        var json = _serviceProvider.GetRequiredService<IJsonService>();
        var files = _serviceProvider.GetRequiredService<IFilesConfig>();

        _logger.Info($"Files:");
        _logger.Info(json.Serialize(files));

        foreach (var entityPipeline in _config.EntityPipelines)
        {
            _logger.Info($"Phase {entityPipeline.Phase}: {entityPipeline.Name}: {entityPipeline.GetType().GetTypeName()}");

            foreach (var stage in stages)
            {
                if (entityPipeline.Stages.TryGetValue(stage, out var steps) && steps.Any())
                {
                    foreach (var step in steps)
                    {
                        _logger.Info($"\t{stage}: {step.StepType.GetTypeName()}");
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}