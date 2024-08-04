namespace PtahBuilder.BuildSystem.Services.Serialization;

public interface IYamlService
{
    T Deserialize<T>(string yaml, YamlDeserializationSettings? settings = null);
    (T entity, Dictionary<string, object>? metadata) DeserializeAndGetMetadata<T>(string yaml, YamlDeserializationSettings? settings = null);
    string Serialize<T>(T entity);
}

public class YamlDeserializationSettings
{
    public Dictionary<string, string> NodeNameToPropertyMappings { get; set; } = new();
}