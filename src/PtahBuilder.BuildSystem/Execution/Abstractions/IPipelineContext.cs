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
    Entity<T> AddEntity(T entity, Dictionary<string, object>? metadata = null);

    void AddValidationError(Entity<T> entity, IStep<T> step, string error);
    void RemoveEntity(Entity<T> entity);
    Entity<T> GetEntity(string id);
}