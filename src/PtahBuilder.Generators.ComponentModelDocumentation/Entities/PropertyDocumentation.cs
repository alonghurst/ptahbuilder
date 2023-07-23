using System.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Entities;

public class PropertyDocumentation
{
    public PropertyDocumentation(PropertyInfo propertyInfo, string displayName, string description)
    {
        PropertyInfo = propertyInfo;
        DisplayName = displayName;
        Description = description;
    }

    public PropertyInfo PropertyInfo { get; }
    public string DisplayName { get; }
    public string Description { get; }
}