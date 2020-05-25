using System.Collections.Generic;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Helpers;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Syntax;

namespace PtahBuilder.BuildSystem.Operations
{
    public class FactoryNamesOperation<T> : Operation<T>
    {
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
}
