using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.BuildSystem.Execution.Abstractions;

public interface IPipelineContext
{
    Task ProcessStepsInStage(Stage stage, ServiceProvider serviceProvider);
    int Phase { get; }

    IEnumerable<(Type type, string id, ValidationError[] errors)> ValidationErrors();
}

public interface IPipelineContext<T> : IPipelineContext
{
    Func<string, string> ProcessId { get; }

    /// <summary>
    /// Adds an entity to the context. An Id is automatically determined or generated depending on the configuration
    /// </summary>
    Entity<T> AddEntity(T entity, Dictionary<string, object>? metadata = null);

    Entity<T> AddEntityWithId(T entity, string id, Dictionary<string, object>? metadata = null);

    void AddValidationError(Entity<T> entity, string source, string error);
    void AddPipelineValidationError(string source, string error);

    void AddValidationError(Entity<T> entity, IStep<T> step, string error) =>
        AddValidationError(entity, step.GetType().GetTypeName(), error);

    void AddPipelineValidationError(IStep<T> step, string error) =>
        AddPipelineValidationError(step.GetType().GetTypeName(), error);

    void RemoveEntity(Entity<T> entity);
    Entity<T> GetEntity(string id);
    bool TryGetEntity(string id, out Entity<T> entity);
}