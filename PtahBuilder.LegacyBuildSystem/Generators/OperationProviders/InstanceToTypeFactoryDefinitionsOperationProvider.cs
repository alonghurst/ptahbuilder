using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Generators.Operations;

namespace PtahBuilder.LegacyBuildSystem.Generators.OperationProviders;

public class InstanceToTypeFactoryDefinitionsOperationProvider<T> : OperationProvider<T>
{
    protected override Operation<T> FinalOperation()
    {
        return new InstanceToTypeFactoryDefinitionsOperation<T>(Context);
    }

    public InstanceToTypeFactoryDefinitionsOperationProvider(IOperationContext<T> context) : base(context)
    {
    }
}