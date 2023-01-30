using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Pipelines;

namespace PtahBuilder.BuildSystem.Execution.Abstractions;

public interface IStage<T>
{
    Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities);
}