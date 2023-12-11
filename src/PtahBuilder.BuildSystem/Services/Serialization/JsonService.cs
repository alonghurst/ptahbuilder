using System.Text.Json;
using System.Text.Json.Serialization;
using PtahBuilder.BuildSystem.Config.Internal;

namespace PtahBuilder.BuildSystem.Services.Serialization;

public class JsonService : IJsonService
{
    public JsonSerializerOptions Options { get; }

    public JsonService(JsonConverterConfig config)
    {
        Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        foreach (var converter in config.Converters)
        {
            Options.Converters.Add(converter);
        }
    }

    public T Deserialize<T>(string text) => JsonSerializer.Deserialize<T>(text, Options) ?? throw new InvalidOperationException("Unable to deserialize text");

    public string Serialize<T>(T entity) => JsonSerializer.Serialize(entity, Options);
}