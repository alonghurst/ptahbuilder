using Humanizer;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class HumanizePropertyStep<T> : IStep<T>
{
    private readonly string _propertyName;

    public HumanizePropertyStep(string propertyName)
    {
        _propertyName = propertyName;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var property = typeof(T).GetProperties().FirstOrDefault(x=>x.Name == _propertyName);

        if (property == null)
        {
            throw new InvalidOperationException($"Unable to find a property on {typeof(T).Name} called \"{_propertyName}\"");
        }

        foreach (var entity in entities)
        {
            var val = property.GetValue(entity.Value);

            if (val != null && val is string strVal)
            {
                property.SetValue(entity.Value, strVal.Humanize());
            }
        }

        return Task.CompletedTask;
    }
}