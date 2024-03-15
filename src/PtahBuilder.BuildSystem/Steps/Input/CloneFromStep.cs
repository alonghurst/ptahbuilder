using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class CloneFromStep<TTarget, TSource> : IStep<TTarget>
{
    private readonly IEntityProvider<TSource> _sourceProvider;

    public CloneFromStep(IEntityProvider<TSource> sourceProvider)
    {
        _sourceProvider = sourceProvider;
    }

    public Task Execute(IPipelineContext<TTarget> context, IReadOnlyCollection<Entity<TTarget>> entities)
    {
        var targetProperties = typeof(TTarget).GetProperties()
            .Where(x => x.CanWrite);
        var sourceProperties = typeof(TSource).GetProperties()
            .Where(x => x.CanWrite);

        var matchedProperties = targetProperties.Select(x =>
            {
                var y = sourceProperties.FirstOrDefault(y => y.Name == x.Name && y.PropertyType == x.PropertyType);

                return new { targetProperty = x, sourceProperty = y };
            })
            .Where(x => x?.sourceProperty != null)
            .Select(x => x!)
            .ToArray();

        foreach (var sourceEntity in _sourceProvider.Entities)
        {
            var targetEntity = Activator.CreateInstance<TTarget>();

            foreach (var matchedProperty in matchedProperties)
            {
                var value = matchedProperty.sourceProperty!.GetValue(sourceEntity.Value.Value);

                matchedProperty.targetProperty.SetValue(targetEntity, value);
            }

            context.AddEntity(targetEntity, new()
            {
                { MetadataKeys.ClonedFrom, sourceEntity.Key }
            });
        }

        return Task.CompletedTask;
    }
}