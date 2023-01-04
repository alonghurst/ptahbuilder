using Newtonsoft.Json;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Generators.Context;

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