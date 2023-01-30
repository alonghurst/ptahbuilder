namespace PtahBuilder.BuildSystem.Execution.Pipelines;

public interface IPipelineContext<in T>
{
    void AddEntity(T entity, Dictionary<string, object> metadata);
}