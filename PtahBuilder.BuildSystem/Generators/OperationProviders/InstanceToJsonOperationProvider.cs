using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;

namespace PtahBuilder.BuildSystem.Generators.OperationProviders;

public class InstanceToJsonOperationProvider<T> : OperationProvider<T>
{
    protected override Operation<T> FinalOperation()
    {
        return new InstanceToJsonProvider<T>(Context);
    }

    public InstanceToJsonOperationProvider(IOperationContext<T> context) : base(context)
    {
    }
}