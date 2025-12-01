using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class ValidationStep<T> : IStep<T>
{
    private readonly Func<T, bool> _predicate;
    private readonly string _message;

    public ValidationStep(Func<T, bool> predicate, string message)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var entity in entities)
        {
            if (_predicate(entity.Value))
            {
                context.AddValidationError(entity, this, _message);
            }
        }

        return Task.CompletedTask;
    }
}

