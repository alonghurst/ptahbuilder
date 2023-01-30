using PtahBuilder.LegacyBuildSystem.FileManagement;
using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Metadata;

namespace PtahBuilder.LegacyBuildSystem.Generators;

public abstract class Operation<T> where T : notnull
{
    private readonly IOperationContext<T> _context;

    public Logger Logger => _context.Logger;
    public PathResolver PathResolver => _context.PathResolver;
    public Dictionary<T, MetadataCollection> Entities => _context.Entities;
    public BaseDataMetadataResolver<T> MetadataResolver => _context.MetadataResolver;
    public SettingsBlob Settings => _context.Settings;

    public virtual int Priority { get; }

    public Operation(IOperationContext<T> context)
    {
        _context = context;
    }
}