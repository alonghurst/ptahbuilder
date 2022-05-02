using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;

namespace PtahBuilder.BuildSystem.Generators;

public abstract class OperationProvider<T> : WithOperationContext<T>
{
    public OperationProvider(IOperationContext<T> context) : base(context)
    {
    }

    public IEnumerable<Operation<T>> BuildOperations()
    {
        yield return new FileMover<T>(Context);

        var operations = GetOperations().ToArray();

        foreach (var operation in operations)
        {
            yield return operation;
        }

        yield return FinalOperation();
    }

    protected abstract Operation<T> FinalOperation();
    
    protected virtual IEnumerable<Operation<T>> GetOperations()
    {
        yield break;
    }
}