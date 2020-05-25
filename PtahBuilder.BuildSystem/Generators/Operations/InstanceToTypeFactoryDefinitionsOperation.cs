using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Helpers;
using PtahBuilder.BuildSystem.Syntax;

namespace PtahBuilder.BuildSystem.Generators.Operations
{
    public class InstanceToTypeFactoryDefinitionsOperation<T> :Operation<T>
    {
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
}
