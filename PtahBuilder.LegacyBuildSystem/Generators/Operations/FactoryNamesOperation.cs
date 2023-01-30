using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Helpers;
using PtahBuilder.LegacyBuildSystem.Metadata;
using PtahBuilder.LegacyBuildSystem.Syntax;

namespace PtahBuilder.LegacyBuildSystem.Generators.Operations;

public class FactoryNamesOperation<T> : Operation<T> where T: notnull
{
    public override int Priority => int.MaxValue;

    public FactoryNamesOperation(IOperationContext<T> context) : base(context)
    {
    }

    [Operate]
    public Dictionary<T, MetadataCollection> Operate(Dictionary<T, MetadataCollection> entities)
    {
        var toConstants = new InstanceToTypeFactoryNamesFileWriter<T>(Logger, MetadataResolver);
        toConstants.Generate(MetadataResolver.AbsoluteNamespaceForOutput, entities.WhereIsNotBuildOnly(), PathResolver.FactoryOutputFile(MetadataResolver, "Names"));

        return entities;
    }
}