namespace PtahBuilder.BuildSystem.Execution;

public interface IPipelineContext<in T>
{
    void AddEntity(T entity, Dictionary<string, object> metadata);
}