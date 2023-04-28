using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Input;

/// <summary>
/// Creates a new instance of an object and uses reflection to copy writable property values from the original entity. Values are copied by reference.
/// The quantityFunc specifies how many instances should be maintained, per entity - a value of 1 will do nothing, a value of 2 will create 1 copy etc.
/// </summary>
public class SimpleCopyStep<T> : IStep<T> where T : class, new()
{
    private readonly Func<T, int> _quantityFunc;

    public SimpleCopyStep(Func<T, int> quantityFunc)
    {
        _quantityFunc = quantityFunc;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var properties = typeof(T).GetProperties().Where(x => x.CanWrite).ToArray();

        foreach (var entity in entities.ToArray())
        {
            var quantity = _quantityFunc.Invoke(entity.Value);

            if (quantity > 1)
            {
                for (int i = 1; i < quantity; i++)
                {
                    var clone = new T();

                    foreach (var property in properties)
                    {
                        var value = property.GetValue(entity.Value);
                        property.SetValue(clone, value);
                    }

                    context.AddEntity(clone);
                }
            }
        }

        return Task.CompletedTask;
    }
}