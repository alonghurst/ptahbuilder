using System;
using System.Collections.Generic;
using System.Text;

namespace PtahBuilder.BuildSystem
{
    public static class ValueParsers
    {
        internal static List<Func<object, object>> UserDefinedValueParsers { get; } = new List<Func<object, object>>();

        public static void AddUserDefinedValueParser(Func<object, object> parser)
        {
            UserDefinedValueParsers.Add(parser);
        }
    }
}
