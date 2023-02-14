using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Input;

public abstract class FilterStep<T>:IStep<T>
{
    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var entity in entities)
        {
            if (ShouldBeFiltered(entity))
            {
                context.RemoveEntity(entity);
            }
        }

        return Task.CompletedTask;
    }

    protected abstract bool ShouldBeFiltered(Entity<T> entity);
}