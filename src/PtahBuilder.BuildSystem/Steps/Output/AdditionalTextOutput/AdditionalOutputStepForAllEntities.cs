using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Output.AdditionalTextOutput;

public abstract class AdditionalOutputStepForAllEntities<T> : AdditionalOutputStep<T>
{
    protected AdditionalOutputStepForAllEntities(string outputDirectory) : base(outputDirectory)
    {
    }

    public override Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var content = GenerateContent(context, entities);

        var filename = CreateFilename(content.filename);

        return File.WriteAllTextAsync(filename, content.content);
    }

    protected abstract (string filename, string content) GenerateContent(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities);
}