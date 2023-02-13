using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class ValidateEntityReferenceStep<TFrom, TTo> : IStep<TFrom>
{
    private readonly IEntityProvider<TTo> _referencing;
    private readonly ILogger _logger;
    private readonly string _propertyName;

    public ValidateEntityReferenceStep(IEntityProvider<TTo> referencing, ILogger logger, string propertyName)
    {
        _referencing = referencing;
        _logger = logger;
        _propertyName = propertyName;
    }

    public Task Execute(IPipelineContext<TFrom> context, IReadOnlyCollection<Entity<TFrom>> entities)
    {
        var property = typeof(TFrom).GetProperty(_propertyName) ?? throw new InvalidOperationException($"Unable to find a property named {_propertyName}");

        foreach (var entity in entities)
        {
            var reference = property.GetValue(entity.Value);

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
        if (!string.IsNullOrWhiteSpace(id) && !_referencing.Entities.ContainsKey(id))
        {
            var error = $"Unable to find a \"{typeof(TTo).Name}\" with Id \"{id}\" ";

            context.AddValidationError(entity, this, error);
        }
    }
}