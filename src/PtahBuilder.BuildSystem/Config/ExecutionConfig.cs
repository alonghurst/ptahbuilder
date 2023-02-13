namespace PtahBuilder.BuildSystem.Config;

public class ExecutionConfig
{
    public ExecutionConfig(FilesConfig filesConfig)
    {
        Files = filesConfig;
    }

    public FilesConfig Files { get; }

    public bool DeleteOutputDirectory { get; set; } = true;

    public Dictionary<Type, PipelineConfig> EntityPipelines { get; } = new();

    public ExecutionConfig AddPipeline<T>(Action<PipelineConfig<T>> configure, string? name = null)
    {
        name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}_Pipeline" : name;

        var pipeline = new PipelineConfig<T>(name);

        configure(pipeline);

        EntityPipelines.Add(typeof(T), pipeline);

        return this;
    }

    public ExecutionConfig AddPipelinePhase(Action<PhaseAddContext> phase)
    {
        var config = new PhaseAddContext();

        phase(config);

        var phaseNumber = EntityPipelines.Any() ? EntityPipelines.Values.Max(x => x.Phase + 1) : 0;
        
        foreach (var pipelineConfig in config.EntityPipelines)
        {
            pipelineConfig.Value.Phase = phaseNumber;

            EntityPipelines.Add(pipelineConfig.Key, pipelineConfig.Value);
        }


        return this;
    }
}