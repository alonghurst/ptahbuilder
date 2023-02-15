namespace PtahBuilder.BuildSystem.Config;

public class PhaseAddContext
{
    public List<PipelineConfig> EntityPipelines { get; } = new();

    public PhaseAddContext AddPipeline<T>(Action<PipelineConfig<T>> configure, string? name = null)
    {
        name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}_Pipeline" : name;

        var pipeline = new PipelineConfig<T>(name);

        configure(pipeline);

        EntityPipelines.Add(pipeline);

        return this;
    }
}