using Humanizer;

namespace PtahBuilder.BuildSystem.Services;

public class EntityMetadataService : IEntityMetadataService
{
    public string GetSimpleName<T>() => GetSimpleName(typeof(T));

    public string GetSimpleName(Type type)
    {
        var name = type.Name;

        if (name.EndsWith("Type"))
        {
            name = name.Substring(0, name.Length - "Type".Length);
        }

        return name;
    }

    public string GetSimpleNamePlural<T>() => GetSimpleNamePlural(typeof(T));

    public string GetSimpleNamePlural(Type type) => GetSimpleName(type).Pluralize();
}