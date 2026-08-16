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
        EntityPipelines.Add(ExecutionConfig.CreateConfiguredPipeline(configure, name));

        return this;
    }
}