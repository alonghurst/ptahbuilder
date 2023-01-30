using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtahBuilder.BuildSystem.Config.Internal
{
    public class CustomValueParserConfig
    {
        public CustomValueParserConfig(Dictionary<Type, Func<object, object>> customValueParsers)
        {
            CustomValueParsers = customValueParsers;
        }

        public Dictionary<Type, Func<object, object>> CustomValueParsers { get; }
    }
}
