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

    public Func<string, string>? DefaultIdProcessor { get; set; }
    public MissingIdPreference? MissingIdPreference { get; set; }

    public List<PipelineConfig> EntityPipelines { get; } = new();

    public List<IDefaultPipelineInjector> DefaultPipelineInjectors { get; } = new();

    public Action<BuilderContext>? PreExecution { get; set; }

    public ExecutionConfig AddDefaultPipelineInjector<T>(Action<PipelineConfig<T>> configure, Type[]? except = null)
    {
        var injector = new DefaultPipelineInjector<T>(configure, except);
        DefaultPipelineInjectors.Add(injector);

        foreach (var pipeline in EntityPipelines)
        {
            injector.Apply(pipeline);
        }

        return this;
    }

    public ExecutionConfig AddPipeline<T>(Action<PipelineConfig<T>> configure, string? name = null)
    {
        EntityPipelines.Add(CreateConfiguredPipeline(configure, name));

        return this;
    }

    internal PipelineConfig<T> CreateConfiguredPipeline<T>(Action<PipelineConfig<T>> configure, string? name = null)
    {
        name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}_Pipeline" : name;

        var pipeline = CreatePipelineConfig<T>(name);

        foreach (var injector in DefaultPipelineInjectors)
        {
            injector.Apply(pipeline);
        }

        configure(pipeline);

        return pipeline;
    }

    public PipelineConfig<T> CreatePipelineConfig<T>(string name)
    {
        var pipeline = new PipelineConfig<T>(name);

        if (DefaultIdProcessor != null && pipeline.ProcessId == null)
        {
            pipeline.ProcessId = DefaultIdProcessor;
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