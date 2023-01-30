using PtahBuilder.LegacyBuildSystem.Generators;
using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Generators.OperationProviders;
using PtahBuilder.LegacyBuildSystem.Generators.Operations;
using PtahBuilder.Tests.LegacyTestBuilder.Types;

namespace PtahBuilder.Tests.LegacyTestBuilder.AdditionalOperations;

public class OperationProviderSample : InstanceToTypeFactoryDefinitionsOperationProvider<SimpleType>
{
    public OperationProviderSample(IOperationContext<SimpleType> context) : base(context)
    {
    }

    protected override IEnumerable<Operation<SimpleType>> GetOperations()
    {
        yield return new OperationSample<SimpleType>(Context);
    }
}

public class OperationSample<T> : Operation<T>
{
    public OperationSample(IOperationContext<T> context) : base(context)
    {
    }

    [Operate]
    public void Operate()
    {
        Logger.Info("I am the operation sample that has been added by an operation provider");
    }
}