using PtahBuilder.BuildSystem.Services.Serialization;

namespace PtahBuilder.BuildSystem.Extensions
{
    public static class DynamicMappingServiceExtensions
    {
        public static void MapColumnsToEntity<T>(this IDynamicMappingService service, T entity, IEnumerable<KeyValuePair<string, int>> mappings, string[] columns) where T : class
        {
            foreach (var mapping in mappings)
            {
                var property = mapping.Key;
                var value = columns[mapping.Value];

                service.Map(entity, property, value);
            }
        }

        public static void MapSeparatedPropertiesToEntity<T>(this IDynamicMappingService service, T entity, string propertyList, char pairSeparator = ',', char valueSeparator = ':') where T : class
        {
            var propertyValues = propertyList.Split(propertyList, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrWhiteSpace(propertyList) && propertyValues.Length == 0)
            {
                propertyValues = new[] { propertyList };
            }

            foreach (var property in propertyValues)
            {
                var split = property.Split(valueSeparator, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length != 2)
                {
                    throw new InvalidOperationException($"Unable to split \"{property}\" into a property / value pair");
                }

                service.Map(entity, split[0], split[1]);
            }
        }
    }
}
