using PtahBuilder.LegacyBuildSystem.FileManagement;
using PtahBuilder.LegacyBuildSystem.Metadata;

namespace PtahBuilder.LegacyBuildSystem.Generators.Context;

public abstract class WithOperationContext<T> where T : notnull
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