using System.Text.Json.Serialization;

namespace PtahBuilder.BuildSystem.Config.Internal;

public class JsonConverterConfig
{
    public JsonConverterConfig(IReadOnlyCollection<JsonConverter> converters)
    {
        Converters = converters;
    }

    public IReadOnlyCollection<JsonConverter> Converters { get; }
}