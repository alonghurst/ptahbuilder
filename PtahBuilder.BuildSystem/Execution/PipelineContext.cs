using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Extensions;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Execution;

public class PipelineContext<T> : IPipelineContext<T>, IEntityProvider<T>
{
    public PipelineConfig<T> Config { get; }
    public Dictionary<string, Entity<T>> Entities { get; } = new();

    private readonly ILogger _logger;
    private readonly IDiagnostics _diagnostics;

    public PipelineContext(PipelineConfig<T> config, ILogger logger, IDiagnostics diagnostics)
    {
        Config = config;

        _logger = logger;
        _diagnostics = diagnostics;
    }

    public void AddEntity(T entity, Dictionary<string, object> metadata)
    {
        var id = Config.GetId(entity);

        var val = new Entity<T>(id, entity, new Metadata(metadata));

        Entities.Add(val.Id, val);

        _logger.Info($"{Config.Name}: Added {val.Id}");
    }

    public async Task ProcessStepsInStage(Stage stage, ServiceProvider serviceProvider)
    {
        if (Config.Stages.TryGetValue(stage, out var steps))
        {
            foreach (var stepConfig in steps)
            {
                await ExecuteStep(serviceProvider, stepConfig);
            }
        }
    }

    private async Task ExecuteStep(ServiceProvider serviceProvider, StepConfig stepConfig)
    {
        var message = $"{Config.Name}: Processing {stepConfig.StepType.GetTypeName()}";

        await _diagnostics.Time(message, async () =>
         {
             var instance = ActivatorUtilities.CreateInstance<IStep<T>>(serviceProvider, stepConfig.Arguments);

             try
             {
                 await instance.Execute(this, Entities.Values);
             }
             catch
             {
                 _logger.Error(message);
                 throw;
             }
         });
    }
}