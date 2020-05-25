using System.Collections.Generic;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Operations;

namespace PtahBuilder.BuildSystem.Generators
{
    public abstract class WithOperationContext<T>
    {
        private readonly IOperationContext<T> _context;

        public Logger Logger => _context.Logger;
        public PathResolver PathResolver => _context.PathResolver;
        public Dictionary<T, MetadataCollection> Entities => _context.Entities;
        public BaseDataMetadataResolver<T> MetadataResolver => _context.MetadataResolver;
        
        public WithOperationContext(IOperationContext<T> context)
        {
            _context = context;
        }
    }
}