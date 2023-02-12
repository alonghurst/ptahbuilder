using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Execution.Abstractions;

public interface IPipelineContext
{
    Task ProcessStepsInStage(Stage stage, ServiceProvider serviceProvider);
    int Phase { get; }
}

public interface IPipelineContext<in T> : IPipelineContext
{
    void AddEntity(T entity, Dictionary<string, object> metadata);
}