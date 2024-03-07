namespace PtahBuilder.BuildSystem.Config;

public class PhaseAddContext
{
    public PhaseAddContext(ExecutionConfig executionConfig)
    {
        _executionConfig = executionConfig;
    }

    public List<PipelineConfig> EntityPipelines { get; } = new();

    private readonly ExecutionConfig _executionConfig;

    public PhaseAddContext AddPipeline<T>(Action<PipelineConfig<T>> configure, string? name = null)
    {
        name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}_Pipeline" : name;

        var pipeline = new PipelineConfig<T>(name);

        if (_executionConfig.DefaultIdGenerator != null)
        {
            pipeline.GenerateId = _executionConfig.DefaultIdGenerator;
        }

        configure(pipeline);

        EntityPipelines.Add(pipeline);

        return this;
    }
}