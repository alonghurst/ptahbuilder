using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Generators;

public class DataGenerator<T> where T : notnull, new()
{
    public PathResolver PathResolver { get; }
    public Logger Logger { get; }
    public BaseDataMetadataResolver<T> MetadataResolver { get; }

    public DataGenerator(Logger logger, PathResolver pathResolver, BaseDataMetadataResolver<T> metadataResolver)
    {
        Logger = logger;
        PathResolver = pathResolver;
        MetadataResolver = metadataResolver;
    }

    public Dictionary<T, MetadataCollection> Generate()
    {
        var dataDirectory = Path.GetFullPath(PathResolver.DataDirectory(MetadataResolver.DataDirectoryToOperateIn));
        if (!Directory.Exists(dataDirectory))
        {
            Logger.Warning($"Unable to find directory {dataDirectory}");
            return new Dictionary<T, MetadataCollection>();
        }

        var tidier = new FileTidier();
        tidier.ParseDirectory(dataDirectory);

        Logger.Info($"Parsing {MetadataResolver.EntityTypeName} data");

        var yaml = new YamlToBaseDataMapper<T>(Logger, MetadataResolver);
        yaml.ParseDirectory(dataDirectory);

        return yaml.ParsedEntitiesMetadata;
    }
}