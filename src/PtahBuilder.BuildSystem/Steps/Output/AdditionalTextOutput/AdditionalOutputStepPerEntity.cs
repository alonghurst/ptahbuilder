using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Output.AdditionalOutput;

public abstract class AdditionalOutputStepPerEntity<T> : AdditionalOutputStep<T>
{
    protected AdditionalOutputStepPerEntity(string outputDirectory) : base(outputDirectory)
    {
    }

    public override async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var tasks = entities
            .Select(x => GenerateContent(context, x))
            .Select(x =>
            {
                var filename = CreateFilename(x.filename);

                return File.WriteAllTextAsync(filename, x.content);
            })
            .ToArray();

        await Task.WhenAll(tasks);
    }

    protected abstract (string filename, string content) GenerateContent(IPipelineContext<T> context, Entity<T> entity);
}