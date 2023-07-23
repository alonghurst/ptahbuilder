using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class InsertEntitiesStep<T> : IStep<T> where T : class
{
    private readonly IEnumerable<T> _entities;
    
    public InsertEntitiesStep(IEnumerable<T> entities)
    {
        _entities = entities;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var entity in _entities)
        {
            context.AddEntity(entity);
        }
        
        return Task.CompletedTask;
    }
}