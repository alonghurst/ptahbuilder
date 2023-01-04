using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Generators.Context;

public class OperationContext<T> : IOperationContext<T> where T : notnull
{
    public Logger Logger { get; }
    public PathResolver PathResolver { get; }
    public Dictionary<T, MetadataCollection> Entities { get; }
    public BaseDataMetadataResolver<T> MetadataResolver { get; }
    public SettingsBlob Settings { get; }

    public OperationContext(Logger logger, PathResolver pathResolver, SettingsBlob settings, BaseDataMetadataResolver<T> metadataResolver, Dictionary<T, MetadataCollection> entities)
    {
        Logger = logger;

        PathResolver = pathResolver;
        Entities = entities;
        Settings = settings;
        MetadataResolver = metadataResolver;
    }
}