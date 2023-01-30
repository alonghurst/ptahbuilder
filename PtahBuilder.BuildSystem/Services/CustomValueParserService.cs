using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public object? TryParseValue(Type destinationType, object value, out bool success)
        {
            if (_config.CustomValueParsers.TryGetValue(destinationType, out var parser))
            {
                var parsed = parser(value);
                success = true;

                return parsed;
            }

            success = false;
            return null;
        }
    }
}
