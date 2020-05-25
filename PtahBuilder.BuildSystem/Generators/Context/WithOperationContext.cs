using System.Collections.Generic;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Operations;

namespace PtahBuilder.BuildSystem.Generators
{
    public abstract class WithOperationContext<T>
    {
        protected IOperationContext<T> Context { get; }

        public Logger Logger => Context.Logger;
        public PathResolver PathResolver => Context.PathResolver;
        public Dictionary<T, MetadataCollection> Entities => Context.Entities;
        public BaseDataMetadataResolver<T> MetadataResolver => Context.MetadataResolver;
        
        public WithOperationContext(IOperationContext<T> context)
        {
            Context = context;
        }
    }
}