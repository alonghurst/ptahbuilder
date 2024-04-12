using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Extensions;
using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Services;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Execution;

public class PipelineContext<T> : IPipelineContext<T>, IEntityProvider<T>
{
    public int Phase => Config.Phase;
    public PipelineConfig<T> Config { get; }
    public Dictionary<string, Entity<T>> Entities { get; } = new();

    private readonly List<ValidationError> _validationErrors = new();

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
        metadata ??= new();

        var id = Config.GetId(entity);

        if (string.IsNullOrWhiteSpace(id))
        {
            var newId = FindBackupId(entity, metadata);

            Config.SetId(entity, newId);
            id = Config.GetId(entity);
        }

        return AddEntityWithId(entity, id, metadata);
    }

    public Entity<T> AddEntityWithId(T entity, string id, Dictionary<string, object>? metadata = null)
    {
        if (Entities.ContainsKey(id))
        {
            switch (Config.DuplicateIdBehaviour)
            {
                case DuplicateIdBehaviour.Throw:
                    throw new InvalidOperationException($"An entity with Id \"{id}\" has already been added");
                case DuplicateIdBehaviour.ReturnExistingEntity:
                    return Entities[id];
                case DuplicateIdBehaviour.GenerateNewId:
                    id = Guid.NewGuid().ToString();

                    break;
            }
        }

        Config.SetId(entity, id);

        metadata ??= new();

        var val = new Entity<T>(id, entity, new(metadata));

        Entities[val.Id] = val;

        _logger.Verbose($"{Config.Name}: Added {val.Id}");

        return val;
    }

    public void AddValidationError(Entity<T> entity, IStep<T> step, string error)
    {
        var name = step.GetType().GetTypeName();

        _logger.Warning($"{entity.Id}: Validation Error - {name}: {error}");

        entity.Validation.Errors.Add(new(name, error));
    }

    public void AddPipelineValidationError(IStep<T> step, string error)
    {
        var name = step.GetType().GetTypeName();

        _logger.Warning($"Validation Error - {name}: {error}");

        _validationErrors.Add(new(name, error));
    }

    public void RemoveEntity(Entity<T> entity)
    {
        Entities.Remove(entity.Id);
    }

    public bool TryGetEntity(string id, out Entity<T> entity)
    {
        if (Entities.TryGetValue(id, out var e))
        {
            entity = e;
            return true;
        }

        entity = null!;
        return false;
    }

    public Entity<T> GetEntity(string id) => Entities[id];

    private string FindBackupId(T entity, Dictionary<string, object> metadata)
    {
        if (Config.MissingIdPreference == MissingIdPreference.SourceFile && metadata.ContainsKey(MetadataKeys.SourceFile))
        {
            var file = metadata[MetadataKeys.SourceFile].ToString();

            return Path.GetFileNameWithoutExtension(file)!.ToSlug();
        }

        foreach (var property in Config.GetIdProperties())
        {
            var value = property?.GetValue(entity)?.ToString() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = Config.GenerateId(value);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

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
        var message = $"{Config.Name}: Processing {stepConfig.StepType.GetTypeName()} ({Entities.Count} entities)";

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
        if (_validationErrors.Any())
        {
            var name = this.GetType().GetTypeName();

            yield return (typeof(T), name, _validationErrors.ToArray());
        }

        foreach (var entity in Entities.Values)
        {
            if (!entity.Validation.IsValid)
            {
                yield return (typeof(T), entity.Id, entity.Validation.Errors.ToArray());
            }
        }
    }
}