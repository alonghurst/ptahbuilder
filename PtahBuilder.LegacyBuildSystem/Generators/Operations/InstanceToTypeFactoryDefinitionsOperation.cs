using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Helpers;
using PtahBuilder.LegacyBuildSystem.Syntax;

namespace PtahBuilder.LegacyBuildSystem.Generators.Operations;

public class InstanceToTypeFactoryDefinitionsOperation<T> :Operation<T>
{
    public override int Priority => int.MaxValue;

    public InstanceToTypeFactoryDefinitionsOperation(IOperationContext<T> context) : base(context)
    {
    }

    [Operate]
    public void Operate()
    {
        var toSyntax = new InstanceToTypeFactoryDefinitionsFileWriter<T>(Logger, MetadataResolver);

        toSyntax.Generate(MetadataResolver.AbsoluteNamespaceForOutput,
            Entities.WhereIsNotBuildOnly(),
            PathResolver.FactoryOutputFile(MetadataResolver, "Types"));
    }
}