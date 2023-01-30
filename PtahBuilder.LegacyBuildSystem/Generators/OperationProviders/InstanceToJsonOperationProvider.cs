using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Generators.Operations;

namespace PtahBuilder.LegacyBuildSystem.Generators.OperationProviders;

public class InstanceToJsonOperationProvider<T> : OperationProvider<T>
{
    protected override Operation<T> FinalOperation()
    {
        return new InstanceToJsonOperation<T>(Context);
    }

    public InstanceToJsonOperationProvider(IOperationContext<T> context) : base(context)
    {
    }
}