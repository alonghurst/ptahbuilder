using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Execution;

public class PipelineContext<T> : IPipelineContext<T>, IEntityProvider<T>
{
    public PipelineConfig<T> Config { get; }
    public Dictionary<string, Entity<T>> Entities { get; } = new();

    public PipelineContext(PipelineConfig<T> config)
    {
        Config = config;
    }
        
    public void AddEntity(T entity, Dictionary<string, object> metadata)
    {
        var id = Config.GetId(entity);

        var val = new Entity<T>(id, entity, new Metadata(metadata));

        Entities.Add(val.Id, val);
    }
}