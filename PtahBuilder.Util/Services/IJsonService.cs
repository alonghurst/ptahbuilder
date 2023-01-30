namespace PtahBuilder.Util.Services;

public interface IJsonService
{
    T Deserialize<T>(string text);
    string Serialize<T>(T entity);
}