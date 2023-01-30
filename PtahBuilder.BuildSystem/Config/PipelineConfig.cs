using System.Reflection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Config;

public abstract class PipelineConfig
{
    public string Name { get; }

    public Dictionary<Stage, List<StepConfig>> Stages { get; } = new();

    public PipelineConfig(string name)
    {
        Name = name;

        foreach (var stage in Enum.GetValues<Stage>())
        {
            Stages.Add(stage, new List<StepConfig>());
        }
    }

    public PipelineConfig Configure(Action<PipelineConfig> configure)
    {
        configure(this);

        return this;
    }


    public PipelineConfig AddStep(Stage stage, Type stageType, params object[] args)
    {
        Stages[stage].Add(new StepConfig(stageType, args));

        return this;
    }
}

public class PipelineConfig<T> : PipelineConfig
{
    public string[] IdProperties { get; set; } = Array.Empty<string>();

    public Func<T, string> GetId { get; set; }

    public PipelineConfig(string name) : base(name)
    {
        GetId = CreateDefaultGetId();
    }

    private Func<T, string> CreateDefaultGetId()
    {
        foreach (var property in GetIdProperties())
        {
            return x => property.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
        }

        return _ => $"{DefaultIdPrefix}_{Guid.NewGuid().ToString()}";
    }

    private const string DefaultIdPrefix = $"DFID_";

    private IEnumerable<PropertyInfo> GetIdProperties()
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
            yield return "Name";
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

    public void SetId(T entity, string id)
    {
        foreach (var propertyInfo in GetIdProperties())
        {
            var current = propertyInfo.GetValue(entity)?.ToString();

            if (string.IsNullOrWhiteSpace(current) || current.StartsWith(DefaultIdPrefix))
            {
                propertyInfo.SetValue(entity, id);
            }
        }
    }

    public PipelineConfig AddStep<TS>(Stage stage, params object[] args) where TS : IStep<T>
    {
        base.AddStep(stage, typeof(TS), args);

        return this;
    }

    public PipelineConfig AddInputStage<TS>(params object[] args) where TS : IStep<T>
    {
        AddStep<TS>(Stage.Input, args);

        return this;
    }

    public PipelineConfig AddOutputStage<TS>(params object[] args) where TS : IStep<T>
    {
        AddStep<TS>(Stage.Output, args);

        return this;
    }

    public PipelineConfig AddProcessStage<TS>(params object[] args) where TS : IStep<T>
    {
        AddStep<TS>(Stage.Process, args);

        return this;
    }

    public PipelineConfig Configure(Action<PipelineConfig<T>> configure)
    {
        configure(this);

        return this;
    }
}