using System.Collections.Generic;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Generators.Operations;
using PtahBuilder.TestBuilder.Types;

namespace PtahBuilder.TestBuilder.AdditionalOperations;

public class OperationProviderSample : OperationProvider<SimpleType>
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