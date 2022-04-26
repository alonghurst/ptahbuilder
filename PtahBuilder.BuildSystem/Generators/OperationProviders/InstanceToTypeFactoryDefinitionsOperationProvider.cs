using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;

namespace PtahBuilder.BuildSystem.Generators.OperationProviders;

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