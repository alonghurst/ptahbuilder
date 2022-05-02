namespace PtahBuilder.BuildSystem.Metadata;

public class MetadataCollection : Dictionary<string, string>
{
    public void TakeUnsetValuesFrom(MetadataCollection parent)
    {
        foreach (var kvp in parent)
        {
            if (!ContainsKey(kvp.Key) && kvp.Key != MetadataKeys.BuildOnly)
            {
                Add(kvp.Key, kvp.Value);
            }
        }
    }

    public string BasedOn
    {
        get
        {
            if (ContainsKey(MetadataKeys.BasedOn))
            {
                return this[MetadataKeys.BasedOn];
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