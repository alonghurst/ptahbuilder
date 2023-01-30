using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.Util.Services;

public class JsonService : IJsonService
{
    public JsonSerializerOptions Options { get; }

    public JsonService()
    {
        Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public T Deserialize<T>(string text) => JsonSerializer.Deserialize<T>(text, Options) ?? throw new InvalidOperationException("Unable to deserialize text");

    public string Serialize<T>(T entity) => JsonSerializer.Serialize(entity, Options);
}