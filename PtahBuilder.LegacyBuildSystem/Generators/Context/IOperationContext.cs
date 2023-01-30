using Newtonsoft.Json;
using PtahBuilder.LegacyBuildSystem.FileManagement;
using PtahBuilder.LegacyBuildSystem.Metadata;

namespace PtahBuilder.LegacyBuildSystem.Generators.Context;

public interface IOperationContext<T>
{
    Logger Logger { get; }
    PathResolver PathResolver { get; }
    Dictionary<T, MetadataCollection> Entities { get; }
    BaseDataMetadataResolver<T> MetadataResolver { get; }
    SettingsBlob Settings { get; }
}

public class SettingsBlob
{
    public JsonSerializerSettings JsonSerializerSettings { get; set; } = new();
}