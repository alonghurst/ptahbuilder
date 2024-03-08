using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Filters;

public class FilterBuildOnlyStep<T> :IStep<T>
{
    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var entity in entities)
        {
            if (entity.Metadata.TryGetValue(MetadataKeys.BuildOnly, out var val) == true)
            {
                context.RemoveEntity(entity);
            }
        }

        return Task.CompletedTask;
    }
}