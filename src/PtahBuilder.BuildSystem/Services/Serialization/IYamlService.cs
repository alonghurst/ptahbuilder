namespace PtahBuilder.BuildSystem.Services.Serialization;

public interface IYamlService
{
    T Deserialize<T>(string yaml, Dictionary<string,string>? nodeNameToPropertyMappings = null);
    (T entity, Dictionary<string, object>? metadata) DeserializeAndGetMetadata<T>(string yaml, Dictionary<string, string>? nodeNameToPropertyMappings = null);
    string Serialize<T>(T entity);
}