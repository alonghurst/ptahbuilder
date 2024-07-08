using PtahBuilder.BuildSystem.Execution;

namespace PtahBuilder.BuildSystem.Config;

public class ExecutionConfig
{
    public ExecutionConfig(FilesConfig filesConfig)
    {
        Files = filesConfig;
    }

    public FilesConfig Files { get; }

    public bool DeleteOutputDirectory { get; set; } = true;
    public bool WriteValidationToTextFile { get; set; } = true;

    public Func<string,string>? DefaultIdGenerator { get; set; }
    public MissingIdPreference? MissingIdPreference { get; set; } 

    public List<PipelineConfig> EntityPipelines { get; } = new();
    
    public Action<BuilderContext>? PreExecution { get; set; }

    public ExecutionConfig AddPipeline<T>(Action<PipelineConfig<T>> configure, string? name = null)
    {
        name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}_Pipeline" : name;

        var pipeline = CreatePipelineConfig<T>(name);

        configure(pipeline);

        EntityPipelines.Add( pipeline);

        return this;
    }

    public PipelineConfig<T> CreatePipelineConfig<T>(string name)
    {
        var pipeline = new PipelineConfig<T>(name);

        if (DefaultIdGenerator != null)
        {
            pipeline.GenerateId = DefaultIdGenerator;
        }

        if (MissingIdPreference.HasValue)
        {
            pipeline.MissingIdPreference = MissingIdPreference.Value;
        }

        return pipeline;
    }

    public ExecutionConfig AddPipelinePhase(Action<PhaseAddContext> phase)
    {
        var config = new PhaseAddContext(this);
        
        phase(config);

        var phaseNumber = EntityPipelines.Any() ? EntityPipelines.Max(x => x.Phase + 1) : 0;
        
        foreach (var pipelineConfig in config.EntityPipelines)
        {
            pipelineConfig.Phase = phaseNumber;

            EntityPipelines.Add(pipelineConfig);
        }


        return this;
    }
}