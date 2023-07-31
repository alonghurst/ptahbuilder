using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class ValidateEntityReferenceStep<TFrom, TTo> : IStep<TFrom>
{
    private readonly IEntityProvider<TTo> _referencing;
    private readonly ILogger _logger;
    private readonly string? _propertyName;
    private readonly Func<TFrom, object?>? _accessor;

    public ValidateEntityReferenceStep(IEntityProvider<TTo> referencing, ILogger logger, string propertyName)
    {
        _referencing = referencing;
        _logger = logger;
        _propertyName = propertyName;
    }

    public ValidateEntityReferenceStep(IEntityProvider<TTo> referencing, ILogger logger, Func<TFrom, object?> accessor)
    {
        _referencing = referencing;
        _logger = logger;
        _accessor = accessor;
    }

    public Task Execute(IPipelineContext<TFrom> context, IReadOnlyCollection<Entity<TFrom>> entities)
    {
        var getter = _accessor ?? 
                     CreatePropertyGetters() ?? 
                     throw new InvalidOperationException($"Unable to create a getter for validating {typeof(TFrom).Name} to {typeof(TTo).Name}");
        
        foreach (var entity in entities)
        {
            var reference = getter(entity.Value);

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

    private Func<TFrom, object?>? CreatePropertyGetters()
    {
        if (string.IsNullOrWhiteSpace(_propertyName))
        {
            return null;
        }

        var property = typeof(TFrom).GetProperty(_propertyName) ?? throw new InvalidOperationException($"Unable to find a property named {_propertyName}");

        return x => property.GetValue(x);
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