using PtahBuilder.BuildSystem.Config.Internal;

namespace PtahBuilder.BuildSystem.Services
{
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

            result = null;
            return false;
        }
    }
}
