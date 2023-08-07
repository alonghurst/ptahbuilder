using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Services.Serialization;

namespace PtahBuilder.BuildSystem.Extensions
{
    public static class DynamicMappingServiceExtensions
    {
        public static void MapColumnsToEntity<T>(this IDynamicMappingService service, T entity, IEnumerable<KeyValuePair<string, int>> mappings, string[] columns) where T : class
        {
            foreach (var mapping  in mappings)
            {
                var property = mapping.Key;
                var value = columns[mapping.Value];

                service.Map(entity, property, value);
            }
        }
    }
}
