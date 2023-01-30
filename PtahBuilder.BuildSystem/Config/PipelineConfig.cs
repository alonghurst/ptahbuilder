using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;

namespace PtahBuilder.BuildSystem.Config;

public abstract class PipelineConfig
{
    public string Name { get; }

    public Dictionary<Stage, List<StageConfig>> Stages { get; } = new();

    public PipelineConfig(string name)
    {
        Name = name;

        foreach (var stage in Enum.GetValues<Stage>())
        {
            Stages.Add(stage, new List<StageConfig>());
        }
    }

    public PipelineConfig Configure(Action<PipelineConfig> configure)
    {
        configure(this);

        return this;
    }


    public PipelineConfig AddStage(Stage stage, Type stageType, params object?[] args)
    {
        Stages[stage].Add(new StageConfig(stageType, args));

        return this;
    }
}

public class PipelineConfig<T> : PipelineConfig
{
    public Func<T, string> GetId { get; set; }

    public PipelineConfig(string name) : base(name)
    {
        GetId = CreateDefaultGetId();
    }

    private Func<T, string> CreateDefaultGetId()
    {
        var properties = typeof(T).GetProperties();

        var entityId = properties.FirstOrDefault(x => x.Name == $"{typeof(T).Name}Id");

        if (entityId != null)
        {
            return x => entityId.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
        }

        var id = properties.FirstOrDefault(x => x.Name == "Id");

        if (id != null)
        {
            return x => id.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
        }

        var typeName = properties.FirstOrDefault(x => x.Name == "TypeName");

        if (typeName != null)
        {
            return x => typeName.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
        }

        var name = properties.FirstOrDefault(x => x.Name == "Name");

        if (name != null)
        {
            return x => name.GetValue(x)?.ToString() ?? throw new InvalidOperationException();
        }

        return _ => Guid.NewGuid().ToString();
    }

    public PipelineConfig AddStage<TS>(Stage stage, params object?[] args) where TS : IStage<T>
    {
        base.AddStage(stage, typeof(TS), args);

        return this;
    }

    public PipelineConfig AddInputStage<TS>(params object?[] args) where TS : IStage<T>
    {
        AddStage<TS>(Stage.Input, args);

        return this;
    }

    public PipelineConfig AddOutputStage<TS>(params object?[] args) where TS : IStage<T>
    {
        AddStage<TS>(Stage.Output, args);

        return this;
    }

    public PipelineConfig AddProcessStage<TS>(params object?[] args) where TS : IStage<T>
    {
        AddStage<TS>(Stage.Process, args);

        return this;
    }

    public PipelineConfig Configure(Action<PipelineConfig<T>> configure)
    {
        return this;
    }
}