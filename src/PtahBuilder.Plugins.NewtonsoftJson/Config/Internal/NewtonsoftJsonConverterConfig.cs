using Newtonsoft.Json;

namespace PtahBuilder.Plugins.NewtonsoftJson.Config.Internal;

public class NewtonsoftJsonConverterConfig
{
    public NewtonsoftJsonConverterConfig(IReadOnlyCollection<JsonConverter> converters)
    {
        Converters = converters;
    }

    public IReadOnlyCollection<JsonConverter> Converters { get; }
}