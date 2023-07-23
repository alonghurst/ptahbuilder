using System.Diagnostics;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

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