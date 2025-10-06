using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Steps.Output.SimpleText;
using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Steps.Input.SimpleText;

public abstract class SimpleTextOutputCreationStep : IStep<SimpleTextOutput>
{
    public Task Execute(IPipelineContext<SimpleTextOutput> context, IReadOnlyCollection<Entity<SimpleTextOutput>> entities)
    {
        foreach (var output in CreateOutput())
        {
            context.AddEntity(output);
        }

        return Task.CompletedTask;
    }

    protected abstract IEnumerable<SimpleTextOutput> CreateOutput();
}