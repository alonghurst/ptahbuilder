using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Entities;

public class TypeDocumentation
{
    public TypeDocumentation(Type type, string displayName, string description, IReadOnlyCollection<PropertyDocumentation> properties)
    {
        Type = type;
        Properties = properties;
        DisplayName = displayName;
        Description = description;
    }

    public string Id => Type.GetTypeName();

    public Type Type { get; }

    public string DisplayName { get; }
    public string Description { get; }

    public IReadOnlyCollection<PropertyDocumentation> Properties { get; }
}