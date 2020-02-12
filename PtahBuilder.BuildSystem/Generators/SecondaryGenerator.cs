using System.Collections.Generic;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Generators
{
    public abstract class SecondaryGenerator<T>
    {
        public Logger Logger { get; }
        public PathResolver PathResolver { get; }
        public Dictionary<T, MetadataCollection> Entities { get; }
        public BaseDataMetadataResolver<T> MetadataResolver { get; }

        public SecondaryGenerator(Logger logger, BaseDataMetadataResolver<T> metadataResolver, PathResolver pathResolver, Dictionary<T, MetadataCollection> entities)
        {
            Logger = logger;

            PathResolver = pathResolver;
            Entities = entities;
            MetadataResolver = metadataResolver;
        }
    }
}