namespace PtahBuilder.BuildSystem.Config;

public class PhaseAddContext
{
    public PhaseAddContext(ExecutionConfig executionConfig)
    {
        ExecutionConfig = executionConfig;
    }

    public List<PipelineConfig> EntityPipelines { get; } = new();

    public ExecutionConfig ExecutionConfig { get; }

    public PhaseAddContext AddPipeline<T>(Action<PipelineConfig<T>> configure, string? name = null)
    {
        name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}_Pipeline" : name;

        var pipeline = ExecutionConfig.CreatePipelineConfig<T>(name);
            
        configure(pipeline);

        EntityPipelines.Add(pipeline);

        return this;
    }
}