using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtahBuilder.BuildSystem.Extensions
{
    public static class CsvExtensions
    {
        public static int ToColumn(this string value)
        {
            return value.ToLower()[0] - 'a';
        }

        public static Dictionary<string, int> ToColumns(this IEnumerable<KeyValuePair<string, string>> columns)
        {
            return columns.ToDictionary(x => x.Key, x => x.Value.ToColumn());
        }
    }
}
