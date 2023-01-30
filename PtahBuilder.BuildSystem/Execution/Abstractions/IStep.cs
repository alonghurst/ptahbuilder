using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Execution.Abstractions;

public interface IStep<T>
{
    Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities);
}