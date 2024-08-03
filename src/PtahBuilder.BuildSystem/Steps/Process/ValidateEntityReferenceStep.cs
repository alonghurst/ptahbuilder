using System.Collections;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Process;


public class ValidationConfig<TFrom>
{
    public bool IsRequired { get; init; }
    public string? PropertyName { get; init; }
    public Func<TFrom, object?>? Accessor { get; init; }
    public Func<string?, bool>? ShouldBeIgnored { get; init; }
}

public class ValidateEntityReferenceStep<TFrom, TTo> : IStep<TFrom>
{
    private readonly IEntityProvider<TTo> _referencing;
    private readonly ILogger _logger;
    private readonly ValidationConfig<TFrom> _config;

    public ValidateEntityReferenceStep(IEntityProvider<TTo> referencing, ILogger logger, ValidationConfig<TFrom> config)
    {
        _referencing = referencing;
        _logger = logger;
        _config = config;
    }

    public ValidateEntityReferenceStep(IEntityProvider<TTo> referencing, ILogger logger, string propertyName, Func<string?, bool>? shouldBeIgnored = null)
        : this(referencing, logger, new()
        {
            PropertyName = propertyName,
            ShouldBeIgnored = shouldBeIgnored
        })
    {

    }

    public ValidateEntityReferenceStep(IEntityProvider<TTo> referencing, ILogger logger, Func<TFrom, object?> accessor, Func<string?, bool>? shouldBeIgnored = null)
        : this(referencing, logger, new()
        {
            Accessor = accessor,
            ShouldBeIgnored = shouldBeIgnored
        })
    {
    }

    public Task Execute(IPipelineContext<TFrom> context, IReadOnlyCollection<Entity<TFrom>> entities)
    {
        var getter = _config.Accessor ??
                     CreatePropertyGetters() ??
                     throw new InvalidOperationException($"Unable to create a getter for validating {typeof(TFrom).Name} to {typeof(TTo).Name}");

        foreach (var entity in entities)
        {
            var reference = getter(entity.Value);

            if (reference is IEnumerable<string> strings)
            {
                foreach (var s in strings)
                {
                    Validate(context, entity, s);
                }
            }
            else if (TryGetDictionaryKeys(reference, out var keys))
            {
                foreach (var s in keys)
                {
                    Validate(context, entity, s);
                }
            }
            else if (reference is string s)
            {
                Validate(context, entity, s);
            }
            else if (reference != null)
            {
                _logger.Warning($"{entity.Id}: Unable to parse \"{reference}\" as an entity reference");
            }
        }

        return Task.CompletedTask;
    }

    private bool TryGetDictionaryKeys(object? reference, out IEnumerable<string> keys)
    {
        if (reference is IDictionary)
        {
            var type = reference.GetType();
            if (type.GetGenericArguments()[0] == typeof(string))
            {
                dynamic dictionary = reference;

                keys = dictionary.Keys;
                return true;
            }
        }

        keys = Enumerable.Empty<string>();
        return false;
    }

    protected virtual Func<TFrom, object?>? CreatePropertyGetters()
    {
        if (string.IsNullOrWhiteSpace(_config.PropertyName))
        {
            return null;
        }

        var property = typeof(TFrom).GetProperty(_config.PropertyName) ?? throw new InvalidOperationException($"Unable to find a property named {_config.PropertyName}");

        return x => property.GetValue(x);
    }

    private void Validate(IPipelineContext<TFrom> context, Entity<TFrom> entity, string id)
    {
        if (_config.ShouldBeIgnored != null && _config.ShouldBeIgnored.Invoke(id))
        {
            return;
        }

        if (_config.IsRequired && string.IsNullOrWhiteSpace(id))
        {
            var error = "Reference is unset but marked as required in configuration";

            context.AddValidationError(entity, this, error);
        }

        if (!string.IsNullOrWhiteSpace(id) && !_referencing.Entities.ContainsKey(id))
        {
            var error = $"Unable to find a {typeof(TTo).Name} with Id \"{id}\" ";

            context.AddValidationError(entity, this, error);
        }
    }
}