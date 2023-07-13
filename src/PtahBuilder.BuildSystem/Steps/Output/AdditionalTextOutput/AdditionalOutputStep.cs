using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Output.AdditionalTextOutput;

public abstract class AdditionalOutputStep<T> : IStep<T>
{
    private readonly string _outputDirectory;

    protected AdditionalOutputStep(string outputDirectory)
    {
        _outputDirectory = outputDirectory;
    }

    public abstract Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities);

    protected string CreateFilename(string filename)
    {
        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
        }

        return Path.Combine(_outputDirectory, filename);
    }
}