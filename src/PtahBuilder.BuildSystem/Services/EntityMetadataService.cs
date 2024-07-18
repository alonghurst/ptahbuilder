using Humanizer;

namespace PtahBuilder.BuildSystem.Services;

public class EntityMetadataService : IEntityMetadataService
{
    private readonly string[] _suffixes =
    {
        "TypeLoad",
        "Type"
    };

    public string GetSimpleName<T>() => GetSimpleName(typeof(T));

    public string GetSimpleName(Type type)
    {
        var name = type.Name;

        foreach (var suffix in _suffixes)
        {
            if (name.EndsWith(suffix))
            {
                name = name.Substring(0, name.Length - suffix.Length);
            }
        }

        return name;
    }

    public string GetSimpleNamePlural<T>() => GetSimpleNamePlural(typeof(T));

    public string GetSimpleNamePlural(Type type) => GetSimpleName(type).Pluralize();
}