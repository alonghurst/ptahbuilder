using System.Collections.Generic;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Helpers;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Syntax;

namespace PtahBuilder.BuildSystem.Operations
{
    public class FactoryNamesOperation<T> : IOperation<T> where T : TypeData
    {
        public Logger Logger { get; }
        public PathResolver PathResolver { get; }
        public BaseDataMetadataResolver<T> MetadataResolver { get; }

        public FactoryNamesOperation(Logger logger, PathResolver pathResolver, BaseDataMetadataResolver<T> metadataResolver)
        {
            Logger = logger;
            PathResolver = pathResolver;
            MetadataResolver = metadataResolver;
        }

        public Dictionary<T, MetadataCollection> Operate(Dictionary<T, MetadataCollection> entities)
        {
            var toConstants = new InstanceToTypeFactoryNamesFileWriter<T>(Logger, MetadataResolver);
            toConstants.Generate(MetadataResolver.AbsoluteNamespaceForOutput, entities.WhereIsNotBuildOnly(), PathResolver.FactoryOutputFile(MetadataResolver, "Names"));

            return entities;
        }
    }
}
