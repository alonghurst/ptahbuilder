using PtahBuilder.BuildSystem.Config.Internal;

namespace PtahBuilder.BuildSystem.Services.Parsing;

public class CustomValueParserService : ICustomValueParserService
{
    private readonly CustomValueParserConfig _config;

    public CustomValueParserService(CustomValueParserConfig config)
    {
        _config = config;
    }

    public bool TryParseValue(Type destinationType, object value, out object? result)
    {
        if (_config.CustomValueParsers.TryGetValue(destinationType, out var parser))
        {
            result = parser(value);
            return true;
        }

        foreach (var (type, p) in _config.CustomValueParsers)
        {
            if (type.IsAssignableFrom(destinationType))
            {
                result = p(value);
                return true;
            }
        }

        result = null;
        return false;
    }
}