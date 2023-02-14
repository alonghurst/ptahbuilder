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
    public int Phase => Config.Phase;
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

    public Entity<T> AddEntity(T entity, Dictionary<string, object>? metadata)
    {
        metadata ??= new Dictionary<string, object>();

        var id = Config.GetId(entity);

        if (string.IsNullOrWhiteSpace(id))
        {
            var newId = FindBackupId(metadata);

            Config.SetId(entity, newId);
            id = Config.GetId(entity);
        }

        var val = new Entity<T>(id, entity, new Metadata(metadata));

        Entities.Add(val.Id, val);

        _logger.Info($"{Config.Name}: Added {val.Id}");

        return val;
    }

    public void AddValidationError(Entity<T> entity, IStep<T> step, string error)
    {
        var name = step.GetType().GetTypeName();

        _logger.Warning($"{entity.Id}: Validation Error - {name}: {error}");

        entity.Validation.Errors.Add(new ValidationError(name, error));
    }

    public void RemoveEntity(Entity<T> entity)
    {
        Entities.Remove(entity.Id);
    }

    private string FindBackupId(Dictionary<string, object> metadata)
    {
        if (metadata.ContainsKey(MetadataKeys.SourceFile))
        {
            var file = metadata[MetadataKeys.SourceFile].ToString();

            return Path.GetFileNameWithoutExtension(file)!;
        }

        throw new InvalidOperationException($"{Config.Name}: Unable to find backup Id");
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
        var message = $"{Config.Name}: Processed {stepConfig.StepType.GetTypeName()}";

        await _diagnostics.Time(message, async () =>
         {
             var instance = (IStep<T>)ActivatorUtilities.CreateInstance(serviceProvider, stepConfig.StepType, stepConfig.Arguments);

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


    public IEnumerable<(Type, string, ValidationError[])> ValidationErrors()
    {
        foreach (var entity in Entities.Values)
        {
            if (!entity.Validation.IsValid)
            {
                yield return (typeof(T), entity.Id, entity.Validation.Errors.ToArray());
            }
        }
    }
}