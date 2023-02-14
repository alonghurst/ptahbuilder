using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Execution.Abstractions;

public interface IPipelineContext
{
    Task ProcessStepsInStage(Stage stage, ServiceProvider serviceProvider);
    int Phase { get; }

    IEnumerable<(Type type, string id, ValidationError[] errors)> ValidationErrors();
}

public interface IPipelineContext<T> : IPipelineContext
{
    void AddEntity(T entity, Dictionary<string, object> metadata);

    void AddValidationError(Entity<T> entity, IStep<T> step, string error);
    void RemoveEntity(Entity<T> entity);
}