using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class ValidateEntityReferenceStep<TFrom, TTo> : IStep<TFrom>
{
    private readonly IEntityProvider<TTo> _referencing;
    private readonly ILogger _logger;
    private readonly Func<TFrom, object?> _getReferencingValue;

    public ValidateEntityReferenceStep(IEntityProvider<TTo> referencing, ILogger logger, Func<TFrom, object?> getReferencingValue)
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
                    Validate(context,entity, s);
                }
            }
            else if (reference is string s)
            {
                Validate(context,entity, s);
            }
            else if (reference != null)
            {
                _logger.Warning($"{entity.Id}: Unable to parse \"{reference}\" as an entity reference");
            }
        }

        return Task.CompletedTask;
    }

    private void Validate(IPipelineContext<TFrom> context, Entity<TFrom> entity, string id)
    {
        if (!_referencing.Entities.ContainsKey(id))
        {
            var error = "Unable to find a \"{typeof(TTo).Name}\" with Id \"{id}\" ";

            context.AddValidationError(entity, this, error);
        }
    }
}