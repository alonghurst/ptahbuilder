using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Config;

public abstract class PipelineConfig
{
    public int Phase { get; set; }

    public string Name { get; }

    public PipelineConfig(string name)
    {
        Name = name;
    }

    public PipelineConfig Configure(Action<PipelineConfig> configure)
    {
        configure(this);

        return this;
    }
}

public class PipelineConfig<T> : PipelineConfig
{
    public Dictionary<Stage, List<IStepConfig<T>>> Stages { get; } = new();

    public DuplicateIdBehaviour DuplicateIdBehaviour { get; set; } = DuplicateIdBehaviour.Throw;

    public string[] IdProperties { get; set; } = Array.Empty<string>();

    public Func<T, Dictionary<string, object>, string> GetId { get; set; }
    public MissingIdPreference MissingIdPreference { get; set; } = MissingIdPreference.FallbackIdProperty;

    public Func<string, string>? ProcessId { get; set; }

    public PipelineConfig(string name) : base(name)
    {
        GetId = CreateDefaultGetId();

        foreach (var stage in Enum.GetValues<Stage>())
        {
            Stages.Add(stage, new());
        }
    }

    protected PipelineConfig AddStepConfig(Stage stage, IStepConfig<T> config)
    {
        Stages[stage].Add(config);

        return this;
    }

    private Func<T, Dictionary<string, object>, string> CreateDefaultGetId()
    {
        foreach (var property in GetIdProperties())
        {
            return (x, _) => property.GetValue(x)?.ToString()?.Replace("'", String.Empty) ?? string.Empty;
        }

        return (_, _) => $"{DefaultIdPrefix}_{Guid.NewGuid().ToString()}";
    }

    private const string DefaultIdPrefix = $"DFID_";

    internal IEnumerable<PropertyInfo> GetIdProperties(bool toUseAsSourceForUnknownId = true)
    {
        var properties = typeof(T).GetProperties();

        IEnumerable<string> PropertyNames()
        {
            foreach (var idProperty in IdProperties)
            {
                yield return idProperty;
            }

            yield return $"{typeof(T).Name}Id";
            yield return "Id";
            yield return "TypeName";

            if (toUseAsSourceForUnknownId)
            {
                yield return "Name";
            }
        }

        foreach (var propertyName in PropertyNames())
        {
            var property = properties.FirstOrDefault(x => x.Name == propertyName);

            if (property != null)
            {
                yield return property;
            }
        }
    }

    public void SetId(T entity, string id, bool force = false)
    {
        foreach (var propertyInfo in GetIdProperties(false))
        {
            var current = propertyInfo.GetValue(entity)?.ToString();

            if (string.IsNullOrWhiteSpace(current) || current.StartsWith(DefaultIdPrefix) || force)
            {
                propertyInfo.SetValue(entity, id);
            }
        }
    }

    public PipelineConfig AddStep<TS>(Stage stage, params object[] args) where TS : IStep<T>
    {
        AddStepConfig(stage, new ActivatedStepConfig<T>(typeof(TS), args));

        return this;
    }

    public PipelineConfig AddInputStep<TS>(params object[] args) where TS : IStep<T>
    {
        AddStep<TS>(Stage.Input, args);

        return this;
    }

    public PipelineConfig AddOutputStep<TS>(params object[] args) where TS : IStep<T>
    {
        AddStep<TS>(Stage.Output, args);

        return this;
    }

    public PipelineConfig AddProcessStep<TS>(params object[] args) where TS : IStep<T>
    {
        AddStep<TS>(Stage.Process, args);

        return this;
    }

    public PipelineConfig AddStep(Stage stage, Func<ServiceProvider, IStep<T>> factory)
    {
        AddStepConfig(stage, new FactoryStepConfig<T>(factory));

        return this;
    }

    public PipelineConfig AddInputStep(Func<ServiceProvider, IStep<T>> factory)
    {
        AddStep(Stage.Input, factory);

        return this;
    }

    public PipelineConfig AddOutputStep(Func<ServiceProvider, IStep<T>> factory)
    {
        AddStep(Stage.Output, factory);

        return this;
    }

    public PipelineConfig AddProcessStep(Func<ServiceProvider, IStep<T>> factory)
    {
        AddStep(Stage.Process, factory);

        return this;
    }

    public PipelineConfig Configure(Action<PipelineConfig<T>> configure)
    {
        configure(this);

        return this;
    }
}

public enum DuplicateIdBehaviour
{
    Throw,
    Replace,
    ReturnExistingEntity,
    GenerateNewId
}

public enum MissingIdPreference
{
    SourceFile, FallbackIdProperty
}