using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.BuildSystem.Config;

public interface IDefaultPipelineInjector
{
    Type BaseType { get; }
    IReadOnlyCollection<Type> Except { get; }
    void Apply(PipelineConfig pipeline);
}

public class DefaultPipelineInjector<T> : IDefaultPipelineInjector
{
    private readonly Action<PipelineConfig<T>> _configure;

    public DefaultPipelineInjector(Action<PipelineConfig<T>> configure, IReadOnlyCollection<Type>? except = null)
    {
        _configure = configure;
        Except = except ?? Array.Empty<Type>();
    }

    public Type BaseType => typeof(T);

    public IReadOnlyCollection<Type> Except { get; }

    public void Apply(PipelineConfig pipeline)
    {
        if (Except.Contains(pipeline.EntityType) || !typeof(T).IsAssignableFrom(pipeline.EntityType))
        {
            return;
        }

        if (pipeline is PipelineConfig<T> exact)
        {
            _configure(exact);
            return;
        }

        var source = new PipelineConfig<T>(pipeline.Name);
        _configure(source);

        typeof(DefaultPipelineInjector<T>)
            .GetMethod(nameof(CopyStepsTo), BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(pipeline.EntityType)
            .Invoke(this, new object[] { source, pipeline });
    }

    private void CopyStepsTo<TDerived>(PipelineConfig<T> source, PipelineConfig pipeline)
        where TDerived : T
    {
        if (pipeline is not PipelineConfig<TDerived> target)
        {
            throw new InvalidOperationException($"Unable to inject default steps into {pipeline.GetType().GetTypeName()}");
        }

        foreach (var (stage, steps) in source.Stages)
        {
            foreach (var step in steps)
            {
                target.AddStepConfig(stage, CreateCompatibleStepConfig<TDerived>(step));
            }
        }
    }

    private static IStepConfig<TDerived> CreateCompatibleStepConfig<TDerived>(IStepConfig<T> stepConfig)
        where TDerived : T
    {
        if (stepConfig is ActivatedStepConfig<T> activated)
        {
            try
            {
                var rebound = RebindStepType(activated.StepType, typeof(TDerived));

                if (rebound != null && typeof(IStep<TDerived>).IsAssignableFrom(rebound))
                {
                    return new ActivatedStepConfig<TDerived>(rebound, activated.Arguments);
                }
            }
            catch (ArgumentException)
            {
                // Generic constraints on the rebound step type were not satisfied
            }
        }

        return new AdaptedStepConfig<TDerived, T>(stepConfig);
    }

    private static Type? RebindStepType(Type stepType, Type derivedType)
    {
        if (!stepType.IsGenericType)
        {
            return stepType;
        }

        var genericArgs = stepType.GetGenericArguments();
        var reboundArgs = genericArgs
            .Select(arg => arg == typeof(T) ? derivedType : arg)
            .ToArray();

        if (reboundArgs.SequenceEqual(genericArgs))
        {
            return stepType;
        }

        return stepType.GetGenericTypeDefinition().MakeGenericType(reboundArgs);
    }
}

internal sealed class AdaptedStepConfig<TDerived, TBase> : IStepConfig<TDerived>
    where TDerived : TBase
{
    private readonly IStepConfig<TBase> _inner;

    public AdaptedStepConfig(IStepConfig<TBase> inner)
    {
        _inner = inner;
    }

    public IStep<TDerived> CreateStep(ServiceProvider serviceProvider) =>
        new AdaptedStep<TDerived, TBase>(_inner.CreateStep(serviceProvider));
}

internal sealed class AdaptedStep<TDerived, TBase> : IStep<TDerived>
    where TDerived : TBase
{
    private readonly IStep<TBase> _inner;

    public AdaptedStep(IStep<TBase> inner)
    {
        _inner = inner;
    }

    public Task Execute(IPipelineContext<TDerived> context, IReadOnlyCollection<Entity<TDerived>> entities)
    {
        var adaptedEntities = entities
            .Select(entity => new Entity<TBase>(entity.Id, entity.Value, entity.Metadata, entity.Validation))
            .ToArray();

        return _inner.Execute(new AdaptedPipelineContext<TDerived, TBase>(context), adaptedEntities);
    }
}

internal sealed class AdaptedPipelineContext<TDerived, TBase> : IPipelineContext<TBase>
    where TDerived : TBase
{
    private readonly IPipelineContext<TDerived> _inner;

    public AdaptedPipelineContext(IPipelineContext<TDerived> inner)
    {
        _inner = inner;
    }

    public int Phase => _inner.Phase;

    public Func<string, string> ProcessId => _inner.ProcessId;

    public Task ProcessStepsInStage(Stage stage, ServiceProvider serviceProvider) =>
        _inner.ProcessStepsInStage(stage, serviceProvider);

    public IEnumerable<(Type type, string id, ValidationError[] errors)> ValidationErrors() =>
        _inner.ValidationErrors();

    public Entity<TBase> AddEntity(TBase entity, Dictionary<string, object>? metadata = null) =>
        Wrap(_inner.AddEntity(Cast(entity), metadata));

    public Entity<TBase> AddEntityWithId(TBase entity, string id, Dictionary<string, object>? metadata = null) =>
        Wrap(_inner.AddEntityWithId(Cast(entity), id, metadata));

    public void AddValidationError(Entity<TBase> entity, string source, string error)
    {
        if (_inner.TryGetEntity(entity.Id, out var original))
        {
            _inner.AddValidationError(original, source, error);
            return;
        }

        _inner.AddPipelineValidationError(source, $"{entity.Id}: {error}");
    }

    public void AddPipelineValidationError(string source, string error) =>
        _inner.AddPipelineValidationError(source, error);

    public void RemoveEntity(Entity<TBase> entity)
    {
        if (_inner.TryGetEntity(entity.Id, out var original))
        {
            _inner.RemoveEntity(original);
        }
    }

    public Entity<TBase> GetEntity(string id) => Wrap(_inner.GetEntity(id));

    public bool TryGetEntity(string id, out Entity<TBase> entity)
    {
        if (_inner.TryGetEntity(id, out var original))
        {
            entity = Wrap(original);
            return true;
        }

        entity = null!;
        return false;
    }

    private static TDerived Cast(TBase entity)
    {
        if (entity is TDerived derived)
        {
            return derived;
        }

        throw new InvalidOperationException($"Entity of type {entity?.GetType().GetTypeName()} cannot be used in a {typeof(TDerived).GetTypeName()} pipeline");
    }

    private static Entity<TBase> Wrap(Entity<TDerived> entity) =>
        new(entity.Id, entity.Value, entity.Metadata, entity.Validation);
}
