namespace PtahBuilder.BuildSystem.Services.Serialization;

public interface IYamlService
{
    T Deserialize<T>(string yaml, YamlDeserializationSettings? settings = null);
    (T entity, Dictionary<string, object>? metadata) DeserializeAndGetMetadata<T>(string yaml, YamlDeserializationSettings? settings = null);
    string Serialize<T>(T entity);
}

public class YamlDeserializationSettings
{
    public UnmatchedPropertyAction UnmatchedPropertyAction { get; set; } = UnmatchedPropertyAction.Throw;

    public Dictionary<string, string> NodeNameToPropertyMappings { get; set; } = new();

    public Dictionary<string, YamlDeserializationPropertySettings> PropertySettings { get; set; } = new();
}

public struct YamlDeserializationPropertySettings
{
    public bool IsIgnored { get; set; }

    public Func<string,string>? PreProcess { get; set; }
}

public enum UnmatchedPropertyAction
{
    Throw,
    Warn
}