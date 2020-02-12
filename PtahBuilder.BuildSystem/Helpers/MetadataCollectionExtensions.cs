using System.Collections.Generic;
using System.Linq;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Helpers
{
    public static class MetadataCollectionExtensions
    {
        public static bool IsBuildOnly(this MetadataCollection metadata)
        {
            if (metadata.TryGetValue(MetadataCollection.BuildOnlyKey, out var val) && val.ToLower() == true.ToString().ToLower())
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<T> WhereIsNotBuildOnly<T>(this Dictionary<T, MetadataCollection> entities) 
        {
            return entities.Where(e => !e.Value.IsBuildOnly())
                .Select(e => e.Key);
        }
    }
}
