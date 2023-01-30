using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Execution;

public interface IStage<T>
{
    Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities);
}