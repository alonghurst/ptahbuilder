using System.ComponentModel.DataAnnotations;
using System.Reflection;
using PtahBuilder.Generators.ComponentModelDocumentation.Abstractions;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Services;

internal class ComponentModelDocumentationProvider : IDocumentationProvider
{
    public (string name, string description) DocumentType(Type type)
    {
        var typeInfo = type.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
        return NameAndDescriptionFromDisplayAttribute(typeInfo, type.GetTypeName());
    }

    public EnumValueDocumentation DocumentEnumValue(Type type, object value)
    {
        var attribute = type.GetCustomAttributeForEnumValue<DisplayAttribute>(value);

        var (name, description) = NameAndDescriptionFromDisplayAttribute(attribute, value.ToString() ?? string.Empty);

        return new EnumValueDocumentation(value.ToString() ?? string.Empty, name, description);
    }

    public PropertyDocumentation DocumentProperty(Type type, PropertyInfo property)
    {
        var propertyInfo = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

        var (propertyName, propertyDescription) = NameAndDescriptionFromDisplayAttribute(propertyInfo, property.Name);

        return new PropertyDocumentation(property, propertyName, propertyDescription);
    }

    private (string name, string description) NameAndDescriptionFromDisplayAttribute(DisplayAttribute? attribute, string name)
    {
        if (attribute?.Name is { } s)
        {
            name = s;
        }

        return (name, attribute?.Description ?? string.Empty);
    }
}