using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Generators;

public abstract class Operation<T>
{
    private readonly IOperationContext<T> _context;

    public Logger Logger => _context.Logger;
    public PathResolver PathResolver => _context.PathResolver;
    public Dictionary<T, MetadataCollection> Entities => _context.Entities;
    public BaseDataMetadataResolver<T> MetadataResolver => _context.MetadataResolver;

    public virtual int Priority { get; }

    public Operation(IOperationContext<T> context)
    {
        _context = context;
    }
}