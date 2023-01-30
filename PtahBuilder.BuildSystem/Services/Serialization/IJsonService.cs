namespace PtahBuilder.BuildSystem.Services.Serialization;

public interface IJsonService
{
    T Deserialize<T>(string text);
    string Serialize<T>(T entity);
}