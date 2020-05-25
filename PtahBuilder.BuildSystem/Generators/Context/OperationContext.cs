using System.Collections.Generic;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Operations
{
    public class OperationContext<T> : IOperationContext<T>
    {
        public Logger Logger { get; }
        public PathResolver PathResolver { get; }
        public Dictionary<T, MetadataCollection> Entities { get; }
        public BaseDataMetadataResolver<T> MetadataResolver { get; }

        public OperationContext(Logger logger, PathResolver pathResolver, BaseDataMetadataResolver<T> metadataResolver, Dictionary<T, MetadataCollection> entities)
        {
            Logger = logger;

            PathResolver = pathResolver;
            Entities = entities;
            MetadataResolver = metadataResolver;
        }
    }
}
