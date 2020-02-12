using System.Collections.Generic;

namespace PtahBuilder.BuildSystem.Metadata
{
    public class MetadataCollection : Dictionary<string, string>
    {
        public const string BuildOnlyKey = "IsBuildOnly";

        public void TakeUnsetValuesFrom(MetadataCollection parent)
        {
            foreach (var kvp in parent)
            {
                if (!ContainsKey(kvp.Key) && kvp.Key != BuildOnlyKey)
                {
                    Add(kvp.Key, kvp.Value);
                }
            }
        }

        public string BasedOn
        {
            get
            {
                if (ContainsKey("BasedOn"))
                {
                    return this["BasedOn"];
                }
                return null;
            }
        }
        public string Animation
        {
            get
            {
                if (ContainsKey("Animation"))
                {
                    return this["Animation"];
                }
                return null;
            }
        }
    }
}
