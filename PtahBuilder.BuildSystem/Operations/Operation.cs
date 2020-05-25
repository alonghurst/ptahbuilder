using System.Collections.Generic;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Operations;

namespace PtahBuilder.BuildSystem.Generators
{
    public abstract class Operation<T>
    {
        private readonly IOperationContext<T> _context;

        public Logger Logger => _context.Logger;
        public PathResolver PathResolver => _context.PathResolver;
        public Dictionary<T, MetadataCollection> Entities => _context.Entities;
        public BaseDataMetadataResolver<T> MetadataResolver => _context.MetadataResolver;

        public virtual int? Priority { get; }

        public Operation(IOperationContext<T> context)
        {
            _context = context;
        }
    }
}