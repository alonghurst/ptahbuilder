using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;

namespace PtahBuilder.BuildSystem.Generators.OperationProviders;

public class InstanceToJsonArrayOperationProvider<T> : OperationProvider<T>
{
    protected override Operation<T> FinalOperation()
    {
        return new InstanceToJsonOperation<T>(Context);
    }

    public InstanceToJsonArrayOperationProvider(IOperationContext<T> context) : base(context)
    {
    }
}