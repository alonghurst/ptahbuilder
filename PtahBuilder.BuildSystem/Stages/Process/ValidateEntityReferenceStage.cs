using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Execution.Pipelines;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Stages.Process;

public class ValidateEntityReferenceStage<TFrom, TTo> : IStage<TFrom>
{
    private readonly IEntityProvider<TTo> _referencing;
    private readonly ILogger _logger;
    private readonly Func<TFrom, object?> _getReferencingValue;

    public ValidateEntityReferenceStage(IEntityProvider<TTo> referencing, ILogger logger, Func<TFrom, object?> getReferencingValue)
    {
        _referencing = referencing;
        _logger = logger;
        _getReferencingValue = getReferencingValue;
    }

    public Task Execute(IPipelineContext<TFrom> context, IReadOnlyCollection<Entity<TFrom>> entities)
    {
        foreach (var entity in entities)
        {
            var reference = _getReferencingValue(entity.Value);

            if (reference is IEnumerable<string> strings)
            {
                foreach (var s in strings)
                {
                    Validate(entity, s);
                }
            }
            else if (reference is string s)
            {
                Validate(entity, s);
            }
            else if (reference != null)
            {
                _logger.Warning($"{entity.Id}: Unable to parse \"{reference}\" as an entity reference");
            }
        }

        return Task.CompletedTask;
    }

    private void Validate(Entity<TFrom> entity, string id)
    {
        if (!_referencing.Entities.ContainsKey(id))
        {
            var error = $"{entity.Id}: Unable to find a \"{typeof(TTo).Name}\" with Id \"{id}\" ";
            _logger.Error(error);

            throw new InvalidOperationException(error);
        }
    }
}